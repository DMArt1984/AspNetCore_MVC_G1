using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Data;
using Microsoft.AspNetCore.Authorization; // Пространство имён, где определён CompanyDbContext

namespace AspNetCore_MVC_Project.Areas.BOX.Controllers
{

    public class ToiRController : Controller
    {

        //public ToiRController()
        //{
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
