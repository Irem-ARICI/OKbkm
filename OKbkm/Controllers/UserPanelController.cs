using Microsoft.AspNetCore.Mvc;

namespace OKbkm.Controllers
{
    public class UserPanelController : Controller
    {
        //public IActionResult Index()
        //{
        //    var name = HttpContext.Session.GetString("NameUsername");
        //    ViewBag.UserName = User.Identity.Name ?? "Kullanıcı";
        //    return View();
        //}

        // Kullanıcı panelinin ana sayfası
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                // Kullanıcı login olmamışsa login sayfasına yönlendir
                return RedirectToAction("Index", "Login");
            }

            ViewBag.UserName = userName;
            return View();
        }

        // Account Create sayfasına yönlendir
        public IActionResult GoToCreateAccount()
        {
            return RedirectToAction("Create", "Account");
        }

        // Transaction sayfasına yönlendir
        public IActionResult GoToTransaction()
        {
            return RedirectToAction("Index", "Transaction");
        }

        // Çıkış yap
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}
