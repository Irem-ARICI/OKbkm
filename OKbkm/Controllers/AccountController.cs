using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace OKbkm.Controllers
{
    public class AccountController : Controller
    {
        private readonly Context _context;

        public AccountController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC))
            {
                return RedirectToAction("Index", "Login");
            }

            var userAccounts = _context.Accounts
                .Where(a => a.TC == userTC)
                .ToList();

            return View(userAccounts); // 🔥 Burası çok önemli
        }
    }
}
