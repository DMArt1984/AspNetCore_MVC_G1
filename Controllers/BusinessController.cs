using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Data;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models.Control;

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
        /// ������ �����������, ���� � �������� �������� ������, � ��������:
        /// - NameController ����� "Business"
        /// - ���� NameArea ����� (�� ������), �� ����� ������ ��������� "Business"
        /// </summary>
        /// <returns>�������� ���� ������ �������� ��� null, ���� ������ ��������.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            // �������� �������� ������������
            var user = await _userManager.GetUserAsync(User);
            if (user?.FactoryId == null)
                return null;

            // ������� �������� ������������ �� FactoryId
            var company = await _context.Factories.FindAsync(user.FactoryId);
            if (company == null)
                return null;

            // ���������, ���� �� � �������� ������ � ������ "Business"
            bool hasAccess = await _context.Purchases
                .Include(p => p.OptionBlock) // ��������� ��������� ������ OptionBlock
                .AnyAsync(p => p.FactoryId == user.FactoryId &&
                               p.OptionBlock.NameController == "Business" &&
                               (string.IsNullOrEmpty(p.OptionBlock.NameArea) || p.OptionBlock.NameArea == "Business"));

            if (!hasAccess)
                return null;

            // ��������� ������ ����������� � ������� �������� ��� ���� ������ ��������.
            // ����� ������������ �������� �������� ��� ������������ ���������� ������ �����������.
            string connectionString = GetCompanyConnectionString($"w{company.Title.Replace(" ", "_")}");
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
            if (dbContext == null)
            {
                // ���� ������� ��� � �������������� ������������ �� ������� ��������
                return RedirectToAction("Index", "Home");
            }

            // ��������� ��� ������ �� ������� Job
            var jobs = await dbContext.Jobs.ToListAsync();
            return View(jobs);
        }
    }
}
