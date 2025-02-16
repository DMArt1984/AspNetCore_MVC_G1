using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.Middleware
{
    /// <summary>
    /// Промежуточное ПО (Middleware) для проверки прав доступа к контроллерам в зависимости от разрешённых модулей компании.
    /// </summary>
    public class ModuleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next; // Делегат для передачи запроса дальше по конвейеру

        /// <summary>
        /// Конструктор принимает следующий обработчик запроса в пайплайне.
        /// </summary>
        public ModuleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Метод обработки HTTP-запроса. Проверяет доступ к контроллерам перед их выполнением.
        /// </summary>
        /// <param name="context">Текущий HTTP-контекст</param>
        public async Task Invoke(HttpContext context)
        {
            // Получаем службы аутентификации и базы данных
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Получаем текущего пользователя
            var user = await userManager.GetUserAsync(context.User);

            // Проверяем, есть ли у пользователя привязанная компания
            if (user != null && user.FactoryId.HasValue)
            {
                // Получаем список разрешённых контроллеров для компании пользователя
                var allowedControllers = await dbContext.Purchases
                    .Where(bm => bm.FactoryId == user.FactoryId)
                    .Select(bm => bm.OptionBlock.NameController) // Теперь берём NameController из OptionBlock
                    .ToListAsync();

                // Разрешаем доступ к главной странице и странице входа/регистрации по умолчанию
                allowedControllers.AddRange(new[] { "Home", "Account" });

                // Получаем маршрут текущего запроса (какой контроллер вызывается)
                var routeValues = context.GetRouteData().Values;
                if (routeValues.ContainsKey("controller"))
                {
                    var controllerName = routeValues["controller"].ToString();

                    // Если у пользователя нет доступа к этому контроллеру, перенаправляем его на главную страницу
                    if (!allowedControllers.Contains(controllerName))
                    {
                        context.Response.Redirect("/Home/Index");
                        return; // Прерываем выполнение запроса
                    }
                }
            }

            // Передаём управление следующему Middleware в конвейере
            await _next(context);
        }
    }
}

