namespace AspNetCore_MVC_Project.Models
{
    /// <summary>
    /// Модель для таблицы Mark, содержащей информацию о базе.
    /// </summary>
    public class Mark
    {
        /// <summary>
        /// Уникальный идентификатор записи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название параметра (например, "Creator" или "Service").
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Значение параметра (например, имя пользователя или "0").
        /// </summary>
        public string Value { get; set; }
    }
}

