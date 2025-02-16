using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.ViewModels
{
    /// <summary>
    /// Модель представления для управления модулями (опциями) компании.
    /// Используется в представлении управления доступом к модулям.
    /// </summary>
    public class OptionsViewModel
    {
        /// <summary>
        /// Идентификатор компании (фабрики).
        /// </summary>
        public int FactoryId { get; set; }

        /// <summary>
        /// Название компании (фабрики).
        /// </summary>
        public string FactoryTitle { get; set; }

        /// <summary>
        /// Список всех доступных модулей (OptionBlocks), которые можно назначить компании.
        /// </summary>
        public List<OptionBlock> AllModules { get; set; } = new();

        /// <summary>
        /// Список модулей (OptionBlocks), которые уже назначены компании.
        /// </summary>
        public List<int> AssignedModules { get; set; } = new();

        /// <summary>
        /// Список модулей, выбранных пользователем для обновления доступа.
        /// </summary>
        public List<int> SelectedModules { get; set; } = new();
    }
}

