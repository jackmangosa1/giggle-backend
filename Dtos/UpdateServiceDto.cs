using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Dtos
{
    public class UpdateServiceDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
        public PriceType PriceType { get; set; }
    }
}
