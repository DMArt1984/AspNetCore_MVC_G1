using System.Collections.Generic;

namespace AspNetCore_MVC_Project.Models
{
    /// <summary>
    /// ������ ��������, �������������� �����������, � ������� ����� ������������ ������������.
    /// ������ �������� ����� ����� ��������� ������������� � ��������� �������.
    /// </summary>
    public class Company
    {
        /// <summary>
        /// ���������� ������������� ��������.
        /// ������������� ������������ ����� ������.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// �������� ��������.
        /// ������ ���� ���������� � �������.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ������������� �������� ��� ������ �������������, ������������� ��������.
        /// ���� �� ������: ���� �������� - ����� �������������.
        /// </summary>
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// ������������� �������� ��� ������ �������, ��������� ��������.
        /// ���� �� ������: ���� �������� - ����� �������.
        /// </summary>
        public virtual ICollection<BuyModule> BuyModules { get; set; } = new List<BuyModule>();
    }
}
