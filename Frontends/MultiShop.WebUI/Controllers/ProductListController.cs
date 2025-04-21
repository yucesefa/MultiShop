using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Controllers
{
    public class ProductListController : Controller
    {

        public IActionResult Index()
        {
            ViewBag.directory1 = "Ana Sayfa";
            ViewBag.directory2 = "Ürünler";
            ViewBag.directory3 = "Ürün Listesi";
            return View();
        }
    }
}
