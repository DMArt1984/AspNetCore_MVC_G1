using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_MVC_Project.Areas.CUBE.Controllers
{
    [Area("CUBE")] // Обязательно
    [AllowAnonymous] // Разрешаем анонимный доступ ко всему контроллеру
    public class WarehouseController : Controller
    {
        public IActionResult Index()
        {
            // Получаем из class TenantResolutionMiddleware
            // Предполагаем, что TenantId - это строка
            var tenantId = HttpContext.Items["TenantId"] as string;
            if (!string.IsNullOrEmpty(tenantId))
            {
                // Используем tenantId в нужной логике
            }

            // Получаем из pattern: "CUBE/{tenant}/{controller=Warehouse}/{action=Index}/{id?}",
            var tenantId2 = RouteData.Values["tenant"];

            return View(tenantId2);
        }
    }
}
