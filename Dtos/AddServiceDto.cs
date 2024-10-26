using ServiceManagementAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class AddServiceDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        [EnumDataType(typeof(PriceType), ErrorMessage = "Invalid price type.")]
        public PriceType PriceType { get; set; }
    }
}
