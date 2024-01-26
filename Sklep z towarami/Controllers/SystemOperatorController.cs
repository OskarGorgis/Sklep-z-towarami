using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep_z_towarami.Data;
using System.Threading.Tasks;

namespace Sklep_z_towarami.Controllers
{
    [Authorize(Roles ="SysOp")]
    public class SystemOperatorController : Controller
    {
        private readonly MyDbContext _context;
        public SystemOperatorController(MyDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _context.Users.ToListAsync();
            ViewBag.usersCount = list.Count;
            return View();
        }
    }
}
