namespace AspNetCore_MVC_Project.Models
{
    /// <summary>
    /// Модель для управления доступными модулями компании.
    /// Определяет, какие контроллеры доступны для компании.
    /// </summary>
    public class BuyModule
    {
        /// <summary>
        /// Уникальный идентификатор записи в таблице BuyModules.
        /// Автоматически генерируется базой данных.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название контроллера, к которому разрешен доступ.
        /// Например, "Business" или "KPI".
        /// </summary>
        public string NameController { get; set; }

        /// <summary>
        /// Идентификатор компании, которой принадлежит модуль.
        /// Может быть null, если модуль не привязан к компании.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с сущностью Company.
        /// Позволяет получить данные о компании, которой принадлежит модуль.
        /// </summary>
        public virtual Company Company { get; set; }
    }
}
