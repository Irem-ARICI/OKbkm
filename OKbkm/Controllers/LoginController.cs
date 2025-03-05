using Microsoft.AspNetCore.Mvc;

namespace YourProject.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string tc, string password)
        {
            if (tc == "12345678901" && password == "1234") // Örnek kontrol
            {
                return RedirectToAction("Welcome");
            }
            ViewBag.Error = "TC Kimlik Numarası veya Şifre hatalı!";
            return View();
        }

        public IActionResult Welcome()
        {
            return Content("Hoşgeldiniz, giriş başarılı!");
        }
    }
}
