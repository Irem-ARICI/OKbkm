using Microsoft.AspNetCore.Mvc;

namespace OKbkm.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
