using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sklep_z_towarami.Data;
using Sklep_z_towarami.Models;

namespace Sklep_z_towarami.Controllers
{
    [AllowAnonymous]
    public class ShopController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult SetBuyer()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true,
            };
            Response.Cookies.Append("Role", "Buyer", cookieOptions);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SetSeller()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true,
            };
            Response.Cookies.Append("Role", "Seller", cookieOptions);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult More(string? id)
        {
            if (Request.Cookies.ContainsKey(id))
            {
                int amount = Convert.ToInt32(Request.Cookies.FirstOrDefault(cookie => cookie.Key == id).Value);
                amount++;
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    IsEssential = true,
                };

                Response.Cookies.Append(id, amount.ToString(), cookieOptions);
            }
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult Less(string? id)
        {
            if (Request.Cookies.ContainsKey(id))
            {
                int amount = Convert.ToInt32(Request.Cookies.FirstOrDefault(cookie => cookie.Key == id).Value);
                amount--;
                if (amount == 0)
                {
                    Response.Cookies.Delete(id);
                }
                else
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsEssential = true,
                    };
                    Response.Cookies.Append(id, amount.ToString(), cookieOptions);
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult Delete(string? id)
        {
            if (Request.Cookies.ContainsKey(id))
            {
                Response.Cookies.Delete(id);
            }
            return RedirectToAction(nameof(Cart));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cart()
        {
            //if (_userManager.)
            var articles = await _context.Articles.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            List<(Article, int)> cartList = new List<(Article, int)> ();

            //Request.Cookies.Join()

            foreach (var article in articles)
            {
                if (Request.Cookies.ContainsKey(article.Name))
                {
                    cartList.Add((article, Convert.ToInt32(Request.Cookies.FirstOrDefault(cookie => cookie.Key == article.Name).Value)));
                }
                
            }

            return View((cartList, categories));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        // POST: Shop/AddToCart
        public IActionResult AddToCart(int itemId)
        {
            int currentCount = 0;
            string articleName = _context.Articles.FirstOrDefault(a => a.Id == itemId).Name;
            if (Request.Cookies.ContainsKey(articleName))
            {
                currentCount = Convert.ToInt32(Request.Cookies[articleName]);
            }
            currentCount++;

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true,
            };

            Response.Cookies.Append(articleName, currentCount.ToString(), cookieOptions);

            return RedirectToAction(nameof(Index));
        }

        // GET: Shop
        public async Task<IActionResult> Index()
        {
            if (_context.Articles == null)
                return Problem("Entity set 'MyDBContext.Articles'  is null.");

            else if (_context.Categories == null)
                return Problem("Entity set 'MyDBContext.Categories'  is null.");

            var articles = await _context.Articles.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            ViewData["Role"] = Request.Cookies.FirstOrDefault(cookie => cookie.Key == "Role").Value;
            if (ViewData["Role"] == null)
                ViewData["Role"] = Request.Cookies.FirstOrDefault(cookie => cookie.Key == "role").Value;

            return View((articles, categories));
        }

        // GET: Shop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Shop/Category/5
        public async Task<IActionResult> Category(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            ViewData["ChosenCategory"] = category.Name;
            ViewData["CategoryId"] = category.Id;

            var articles = await _context.Articles.Where(a => a.CategoryId == id).ToListAsync();
            var categories = await _context.Categories.ToListAsync();


            return View((articles, categories));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
