using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sklep_z_towarami.Data;
using Sklep_z_towarami.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Sklep_z_towarami.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CheckoutController
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            List<(Article, int)> cartList = new List<(Article, int)>();

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

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([Bind("Name,LastName,Street,City,PostalCode,PaymentMethod")] Order order)
        {

            var articles = await _context.Articles.ToListAsync();
            foreach (var article in articles)
            {
                if (Request.Cookies.ContainsKey(article.Name))
                {
                    Response.Cookies.Delete(article.Name);
                }

            }

            return View(order);
        }
    }
}