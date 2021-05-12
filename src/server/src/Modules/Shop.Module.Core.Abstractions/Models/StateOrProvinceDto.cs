namespace Shop.Module.Core.Models
{
    public class StateOrProvinceDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public StateOrProvinceLevel Level { get; set; }
    }
}
