namespace AspNetCore_MVC_Project.Models
{
    public class BuyModule
    {
        public int Id { get; set; }
        public string NameController { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
