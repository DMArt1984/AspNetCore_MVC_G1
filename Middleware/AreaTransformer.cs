using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCore_MVC_Project.Middleware
{
    public class AreaTransformer : DynamicRouteValueTransformer
    {
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            // Если в маршруте не задан параметр area, определяем его по имени контроллера
            if (!values.ContainsKey("area") && values.TryGetValue("controller", out var controller))
            {
                string controllerName = controller.ToString();

                // Здесь реализуем логику сопоставления: если имя контроллера "KPI" – area "KPI", если "OM" – area "OM"
                if (controllerName.Equals("KPI", StringComparison.OrdinalIgnoreCase))
                {
                    values["area"] = "KPI";
                }
                else if (controllerName.Equals("OM", StringComparison.OrdinalIgnoreCase))
                {
                    values["area"] = "OM";
                }
                // При необходимости можно добавить дополнительные условия для других контроллеров.
            }

            return new ValueTask<RouteValueDictionary>(values);
        }
    }
}
