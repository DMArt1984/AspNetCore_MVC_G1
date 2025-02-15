using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_MVC_Project.Middleware
{
    public class ModuleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public ModuleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            var user = await userManager.GetUserAsync(context.User);

            if (user != null && user.CompanyId.HasValue)
            {
                var allowedControllers = await dbContext.BuyModules
                    .Where(bm => bm.CompanyId == user.CompanyId)
                    .Select(bm => bm.NameController)
                    .ToListAsync();

                allowedControllers.AddRange(new[] { "Home", "Account" }); // Всегда разрешены

                var routeValues = context.GetRouteData().Values;
                if (routeValues.ContainsKey("controller"))
                {
                    var controllerName = routeValues["controller"].ToString();
                    if (!allowedControllers.Contains(controllerName))
                    {
                        context.Response.Redirect("/Home/Index");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
