using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_MVC_Project.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;

        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected string GetCompanyConnectionString(string databaseName)
        {
            string template = _configuration.GetConnectionString("CompanyDatabaseTemplate");
            return string.Format(template, databaseName);
        }
    }
}
