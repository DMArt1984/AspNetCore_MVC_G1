using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.ViewModels
{

    public class OptionsViewModel
    {
        public int FactoryId { get; set; }
        public string FactoryName { get; set; }
        public List<OptionBlock> AllModules { get; set; } = new();
        public List<int> AssignedModules { get; set; } = new(); //
        public List<int> SelectedModules { get; set; } = new(); //
    }

}
