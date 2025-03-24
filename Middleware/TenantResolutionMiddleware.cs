namespace AspNetCore_MVC_Project.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Предположим, что URL выглядит как: https://localhost:64820/CUBE/Apple/...
            // Разбиваем путь запроса на сегменты
            var segments = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            // Проверяем, что первый сегмент равен "CUBE"
            if (segments.Length > 0 && segments[0].Equals("CUBE", StringComparison.OrdinalIgnoreCase))
            {
                // Если есть второй сегмент, считаем его tenantId
                if (segments.Length > 1)
                {
                    var tenantId = segments[1];
                    if (!string.IsNullOrEmpty(tenantId))
                    {
                        context.Items["TenantId"] = tenantId;
                    }
                }
            }
            await _next(context);
        }
    }
}
