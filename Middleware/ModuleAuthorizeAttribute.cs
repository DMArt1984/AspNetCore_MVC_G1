// ModuleAuthorizeAttribute.cs
// Атрибут, ограничивающий доступ к контроллеру или действию на основе купленных модулей

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.Models.Control;

/// <summary>
/// Атрибут авторизации, ограничивающий доступ к контроллерам или действиям
/// на основе купленных модулей предприятия (OptionBlock).
/// </summary>
public class ModuleAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _moduleName;

    /// <summary>
    /// Создает атрибут с указанием имени модуля (контроллера), к которому должен быть доступ.
    /// </summary>
    /// <param name="moduleName">Имя контроллера, соответствующее значению OptionBlock.NameController</param>
    public ModuleAuthorizeAttribute(string moduleName)
    {
        _moduleName = moduleName;
    }

    /// <summary>
    /// Метод фильтра, вызываемый до выполнения действия контроллера.
    /// Проверяет, имеет ли текущий пользователь доступ к указанному модулю.
    /// </summary>
    /// <param name="context">Контекст выполнения действия</param>
    /// <param name="next">Делегат для вызова следующего обработчика</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        // Проверка, что пользователь аутентифицирован
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ForbidResult(); // Возвращаем 403
            return;
        }

        // Получение сервисов из DI
        var db = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

        // Получение текущего пользователя
        var currentUser = await userManager.GetUserAsync(user);
        if (currentUser == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Проверка: куплен ли модуль соответствующим предприятием
        var hasModule = await db.Purchases
            .Include(p => p.OptionBlock)
            .AnyAsync(p => p.FactoryId == currentUser.FactoryId &&
                           p.OptionBlock.NameController.ToLower() == _moduleName.ToLower());

        if (!hasModule)
        {
            context.Result = new ForbidResult(); // Модуль не доступен — 403
            return;
        }

        // Все в порядке — передаем выполнение дальше
        await next();
    }
}
