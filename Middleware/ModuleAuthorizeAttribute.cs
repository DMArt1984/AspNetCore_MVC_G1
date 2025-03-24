// ModuleAuthorizeAttribute.cs
// Атрибут, ограничивающий доступ к контроллеру или действию на основе купленных модулей

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.Models.Control;

public class ModuleAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _moduleName;

    public ModuleAuthorizeAttribute(string moduleName)
    {
        _moduleName = moduleName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ForbidResult();
            return;
        }

        var db = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

        var currentUser = await userManager.GetUserAsync(user);
        if (currentUser == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasModule = await db.Purchases
            .Include(p => p.OptionBlock)
            .AnyAsync(p => p.FactoryId == currentUser.FactoryId &&
                           p.OptionBlock.NameController.ToLower() == _moduleName.ToLower());

        if (!hasModule)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}

