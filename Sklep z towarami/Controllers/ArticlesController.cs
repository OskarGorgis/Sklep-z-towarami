using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Sklep_z_towarami.Data;
using Sklep_z_towarami.Models;


namespace Sklep_z_towarami.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly MyDbContext _context;

        public ArticlesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            //var articlesDbContext = _context.Articles.Include(a => a.Category);
            ViewBag.Categories = _context.Categories.Select(c => c.Name).ToList();

            return View(await _context.Articles.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["Category"] = _context.Categories.Where(c => c.Id == article.CategoryId).ToList()[0].Name;
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,File,ImagePath,CategoryId")] Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (article.File != null)
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload");
                        if (!Directory.Exists(path))
                        {
                           Directory.CreateDirectory(path);
                        }
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(article.File.FileName)}";
                        string filePath = Path.Combine("./wwwroot/upload", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await article.File.CopyToAsync(stream);
                        }
                        article.ImagePath = Path.Combine("./upload", fileName);
                    }

                    _context.Add(article);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(nameof(Index));
            }
            catch (DbUpdateException) 
            {
                TempData["Error"] = "Unknown error";
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,ImagePath,CategoryId, Promo")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(article.ImagePath))
            {
                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), article.ImagePath.TrimStart('.'));
                var path = Path.GetFullPath("wwwroot");
                System.IO.File.Delete(Path.Combine(path, article.ImagePath));
                
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Filter(string categoryName)
        {
            ViewBag.Categories = _context.Categories.Select(c => c.Name).ToList();
            if (categoryName != "" && categoryName != null)
            {
                return View("Index", await _context.Articles.Where(a => a.Category.Name == categoryName).ToListAsync());
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
