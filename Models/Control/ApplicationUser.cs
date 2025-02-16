using Microsoft.AspNetCore.Identity;

namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// ����������� ������ ������������ ��� ������� ��������������.
    /// ����������� �� IdentityUser, �������� ����� � ���������.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// ������������� ��������, � ������� ����������� ������������.
        /// ����� ���� null, ���� ������������ �� �������� � ���������� ��������.
        /// </summary>
        public int? FactoryId { get; set; }

        /// <summary>
        /// ������������� �������� ��� ����� � ��������� Company.
        /// ��������� �������� ������ � ��������, � ������� ����������� ������������.
        /// </summary>
        public virtual Factory Factory { get; set; }
    }
}
