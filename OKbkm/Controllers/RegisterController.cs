using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace OKbkm.Controllers
{
    public class RegisterController : Controller
    {
        private readonly Context _context;

        public RegisterController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Register model)
        {
            Console.WriteLine("📌 Register formu gönderildi!");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("🚨 ModelState geçersiz! Form validation hatası var.");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Hata - {key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            Console.WriteLine($"Gelen TC Kimlik No: {model.TC}");
            Console.WriteLine($"Gelen Ad Soyad: {model.NameUsername}");
            Console.WriteLine($"Gelen Mail: {model.Mail}");
            Console.WriteLine($"Gelen Şifre: {model.Password}");
            Console.WriteLine($"Gelen Telefon: {model.PhoneNumber}");
            Console.WriteLine($"Gelen Adres: {model.Address}");

            var existingUser = _context.Registers.FirstOrDefault(u => u.TC == model.TC);
            if (existingUser != null)
            {
                Console.WriteLine("🚨 Bu TC kimlik numarası ile zaten kayıtlı bir kullanıcı var!");
                ModelState.AddModelError("", "Bu TC kimlik numarası ile zaten kayıtlı bir kullanıcı var.");
                return View(model);
            }

            _context.Registers.Add(model);
            int result = _context.SaveChanges();
            Console.WriteLine($"✅ Kayıt işlemi tamamlandı! Kaydedilen kayıt sayısı: {result}");

            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome()      // kaldırcam bunu büyük ihtimalle
        {
            return Content("Kaydınız başarıyla tamamlandı. Hoş geldiniz!");
        }
    }
}