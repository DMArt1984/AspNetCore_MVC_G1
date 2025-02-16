namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// Модель для управления доступными модулями компании.
    /// Определяет, какие контроллеры доступны для компании.
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// Уникальный идентификатор записи в таблице BuyModules.
        /// Автоматически генерируется базой данных.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор компании, которой принадлежит модуль.
        /// Может быть null, если модуль не привязан к компании.
        /// </summary>
        public int? FactoryId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с сущностью Company.
        /// Позволяет получить данные о компании, которой принадлежит модуль.
        /// </summary>
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Идентификатор связанного блока опций.
        /// </summary>
        public int OptionBlockId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с `OptionBlock`.
        /// </summary>
        public virtual OptionBlock OptionBlock { get; set; }
    }

}
