namespace AspNetCore_MVC_Project.Models.Control
{
    public class OptionBlock
    {
        /// <summary>
        /// Уникальный идентификатор записи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название контроллера, к которому разрешен доступ.
        /// Например, "Business" или "KPI".
        /// </summary>
        public string? NameController { get; set; }

        /// <summary>
        /// Название области, к которой разрешен доступ.
        /// Например, "BOX" или "CUBE".
        /// </summary>
        public string? NameArea { get; set; }

        /// <summary>
        /// Навигационное свойство - список покупок, связанных с этим блоком.
        /// </summary>
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }

}
