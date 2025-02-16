namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// ������ ��� ���������� ���������� �������� ��������.
    /// ����������, ����� ����������� �������� ��� ��������.
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// ���������� ������������� ������ � ������� BuyModules.
        /// ������������� ������������ ����� ������.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// �������� �����������, � �������� �������� ������.
        /// ��������, "Business" ��� "KPI".
        /// </summary>
        public string NameController { get; set; }

        /// <summary>
        /// ������������� ��������, ������� ����������� ������.
        /// ����� ���� null, ���� ������ �� �������� � ��������.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// ������������� �������� ��� ����� � ��������� Company.
        /// ��������� �������� ������ � ��������, ������� ����������� ������.
        /// </summary>
        public virtual Company Company { get; set; }
    }
}
