using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
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
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.SelectMany(kvp => kvp.Value.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                ViewBag.Errors = allErrors;
                return View(model);
            }

            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            //    ViewBag.Errors = errors; // View’a gönder
            //    return View(model);
            //}

            var existingUser = _context.Registers.FirstOrDefault(u => u.TC == model.TC);
            if (existingUser != null)
            {
                ModelState.AddModelError("TC", "Bu TC kimlik numarası ile zaten kayıtlı bir kullanıcı var.");
                return View(model);
            }

            _context.Registers.Add(model);
            int result = _context.SaveChanges();

            if (result > 0)
            {
                TempData["Success"] = "Kayıt başarıyla tamamlandı!";
                return RedirectToAction("Welcome");
            }

            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
            return View(model);
        }

        public IActionResult Welcome()
        {
            return Content("✅ Kaydınız başarıyla tamamlandı. Hoş geldiniz!");
        }
    }
}
