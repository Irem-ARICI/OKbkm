using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace OKbkm.Controllers
{
    public class LoginController : Controller
    {
        private readonly Context _context;

        public LoginController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login model)
        {
            //Console.WriteLine("📌 Login formu gönderildi!");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("🚨 ModelState geçersiz! Form validation hatası var.");
                return View(model);
            }

            Console.WriteLine($"📌 Girilen TC: {model.TC}");
            Console.WriteLine($"📌 Girilen Şifre: {model.Password}");

            var user = _context.Registers.FirstOrDefault(u => u.TC == model.TC && u.Password == model.Password);
            if (user != null)
            {
                Console.WriteLine("✅ Giriş başarılı!");

                HttpContext.Session.SetString("UserTC", user.TC); // Kullanıcı bilgilerini session'a kaydet
                HttpContext.Session.SetString("UserName", user.NameUsername);

                // Giriş yapan kullanıcıyı Logins tablosuna ekleyelim
                var loginEntry = new Login
                {
                    TC = user.TC,
                    Password = user.Password,
                    LoginDate = DateTime.UtcNow
                };
                _context.Logins.Add(loginEntry);
                _context.SaveChanges();

                return RedirectToAction("Index", "UserPanel");
            }

            Console.WriteLine("🚨 Giriş başarısız! TC kimlik numarası veya şifre hatalı.");
            ViewBag.Error = "TC kimlik numarası veya şifre hatalı!";
            return View(model);
        }

        public IActionResult Welcome()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserTC")))
            {
                return RedirectToAction("Index");
            }
            return Content($"Hoşgeldiniz, {HttpContext.Session.GetString("UserName")}!");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Kullanıcıyı çıkış yaptır
            return RedirectToAction("Index");
        }
    }
}
