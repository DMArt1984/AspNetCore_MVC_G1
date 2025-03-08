using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_MVC_Project.Areas.CUBE.Controllers
{
    public class WarehouseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
