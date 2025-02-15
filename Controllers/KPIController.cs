using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_MVC_Project.Controllers
{
    [Authorize]
    public class KPIController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public KPIController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
            : base(configuration)
        {
            _userManager = userManager;
            _context = context;
        }

        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return null;

            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return null;

            bool hasAccess = await _context.BuyModules.AnyAsync(bm => bm.CompanyId == user.CompanyId && bm.NameController == "KPI");
            if (!hasAccess) return null;

            string connectionString = GetCompanyConnectionString($"w{company.Name.Replace(" ", "_")}");
            return new CompanyDbContext(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null) return RedirectToAction("Index", "Home");

            var statistics = await dbContext.Statistics.ToListAsync();
            return View(statistics);
        }
    }
}