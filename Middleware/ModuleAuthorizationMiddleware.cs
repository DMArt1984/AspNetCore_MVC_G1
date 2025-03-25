
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.Middleware
{
    /// <summary>
    /// Промежуточное ПО (Middleware) для проверки прав доступа к контроллерам и областям
    /// в зависимости от разрешенных модулей компании.
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
        /// Метод обработки HTTP-запроса. Проверяет, имеет ли текущий пользователь доступ к заданной паре Area/Controller.
        /// </summary>
        /// <param name="context">Текущий HTTP-контекст</param>
        public async Task Invoke(HttpContext context)
        {
            // Получаем службы аутентификации и базы данных
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Получаем текущего пользователя
            var user = await userManager.GetUserAsync(context.User);

            // Если пользователь аутентифицирован и у него задан FactoryId
            if (user != null && user.FactoryId.HasValue)
            {
                // Получаем список разрешенных модулей для компании пользователя.
                // Здесь для каждого элемента выбирается пара (Controller, Area) из связанной сущности OptionBlock.
                // Если OptionBlock.NameArea хранит название области, то оно будет использоваться для проверки.
                var allowedModules = await dbContext.Purchases
                    .Where(p => p.FactoryId == user.FactoryId)
                    .Select(p => new {
                        Controller = p.OptionBlock.NameController,
                        Area = p.OptionBlock.NameArea
                    })
                    .ToListAsync();

                // Добавляем стандартные контроллеры, доступ к которым разрешен вне зависимости от области.
                // При этом Area оставляем null или пустой, чтобы проверка разрешала доступ независимо от области.
                allowedModules.Add(new { Controller = "Home", Area = (string)null });
                allowedModules.Add(new { Controller = "Account", Area = (string)null });
                allowedModules.Add(new { Controller = "Admin", Area = (string)null }); // временное решение для Admin

                // Сохраняем список разрешенных модулей в HttpContext для дальнейшего использования (например, для формирования меню)
                context.Items["AllowedControllers"] = allowedModules;

                // Получаем текущие значения маршрута из RouteData
                var routeValues = context.GetRouteData().Values;

                if (routeValues.ContainsKey("controller"))
                {
                    var controllerName = routeValues["controller"].ToString();
                    // Если в маршруте не указана область, считаем ее пустой
                    var areaName = routeValues.ContainsKey("area") ? routeValues["area"].ToString() : string.Empty;

                    // Проверяем, разрешен ли доступ на основе списка разрешенных модулей.
                    // Логика следующая:
                    // 1. Если разрешенный модуль указывает только область (Controller пустой, Area задана),
                    //    то доступ разрешается, если в маршруте указана совпадающая область.
                    // 2. Если разрешенный модуль указывает как контроллер, так и область,
                    //    то проверяем, что оба совпадают с параметрами маршрута.
                    // 3. Если разрешенный модуль не задает область (Area пустая), но задает контроллер,
                    //    то проверяем только контроллер.
                    bool allowed = allowedModules.Any(m =>
                    {
                        string allowedController = m.Controller as string;
                        string allowedArea = m.Area as string;

                        // Вариант 1: Только область задана
                        if (string.IsNullOrWhiteSpace(allowedController) && !string.IsNullOrWhiteSpace(allowedArea))
                        {
                            return string.Equals(allowedArea, areaName, StringComparison.OrdinalIgnoreCase);
                        }

                        // Вариант 2: Оба поля заданы
                        if (!string.IsNullOrWhiteSpace(allowedController) && !string.IsNullOrWhiteSpace(allowedArea))
                        {
                            return string.Equals(allowedController, controllerName, StringComparison.OrdinalIgnoreCase)
                                   && string.Equals(allowedArea, areaName, StringComparison.OrdinalIgnoreCase);
                        }

                        // Вариант 3: Задан только контроллер (область не указана в разрешениях)
                        if (!string.IsNullOrWhiteSpace(allowedController) && string.IsNullOrWhiteSpace(allowedArea))
                        {
                            return string.Equals(allowedController, controllerName, StringComparison.OrdinalIgnoreCase);
                        }

                        // Если ни контроллер, ни область не заданы — считаем, что это невалидная запись
                        return false;
                    });

                    // Если доступа к указанной паре (Area/Controller) нет, перенаправляем пользователя на главную страницу.
                    if (!allowed)
                    {
                        context.Response.Redirect("/Home/Index");
                        return; // Прерываем выполнение запроса
                    }
                }
            }

            // Передаем управление следующему Middleware в конвейере
            await _next(context);
        }
    }
}
