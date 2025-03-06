using Microsoft.AspNetCore.Mvc;

namespace OKbkm.Controllers
{
    public class TransactionHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
