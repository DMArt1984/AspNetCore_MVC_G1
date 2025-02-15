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
    /// <summary>
    /// ���������� ��� ������ � ������-�������.
    /// �������� �� ������ ������������� � ������ ������� Job.
    /// </summary>
    [Authorize] // ������������ ������ ������ ��� �������������� �������������
    public class BusinessController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// ����������� ����������� Business.
        /// </summary>
        /// <param name="userManager">�������� ������������� Identity.</param>
        /// <param name="context">�������� �������� ���� ������.</param>
        /// <param name="configuration">������������ ����������.</param>
        public BusinessController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
            : base(configuration) // �������� ����������� BaseController
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// �������� �������� ���� ������ ��������.
        /// ���������, ���� �� � ������������ ������ � ������ "Business".
        /// </summary>
        /// <returns>�������� ���� ������ �������� ��� null, ���� ������ ��������.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            // �������� �������� ������������
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return null;

            // ������� �������� ������������
            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return null;

            // ���������, ���� �� � �������� ������ � ������ "Business"
            bool hasAccess = await _context.BuyModules.AnyAsync(bm => bm.CompanyId == user.CompanyId && bm.NameController == "Business");
            if (!hasAccess) return null;

            // ��������� ������ ����������� � ������� �������� ��� ���� ������ ��������
            string connectionString = GetCompanyConnectionString($"w{company.Name.Replace(" ", "_")}");
            return new CompanyDbContext(connectionString);
        }

        /// <summary>
        /// ���������� ������ ������ �� ������� Job.
        /// �������� ������ �������������, � ������� ���� ������ � ������ "Business".
        /// </summary>
        /// <returns>�������� � �������� ������ Job.</returns>
        public async Task<IActionResult> Index()
        {
            // �������� �������� ���� ������ ��������
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null) return RedirectToAction("Index", "Home"); // ���� ������� ��� � �������������� �� ������� ��������

            // ��������� ��� ������ �� ������� Job
            var jobs = await dbContext.Jobs.ToListAsync();
            return View(jobs);
        }
    }
}
