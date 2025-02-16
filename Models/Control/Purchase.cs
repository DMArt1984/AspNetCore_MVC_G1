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
        /// ������������� ��������, ������� ����������� ������.
        /// ����� ���� null, ���� ������ �� �������� � ��������.
        /// </summary>
        public int? FactoryId { get; set; }

        /// <summary>
        /// ������������� �������� ��� ����� � ��������� Company.
        /// ��������� �������� ������ � ��������, ������� ����������� ������.
        /// </summary>
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// ������������� ���������� ����� �����.
        /// </summary>
        public int OptionBlockId { get; set; }

        /// <summary>
        /// ������������� �������� ��� ����� � `OptionBlock`.
        /// </summary>
        public virtual OptionBlock OptionBlock { get; set; }
    }

}
