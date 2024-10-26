using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class UpdateProviderProfileDto
    {
        [StringLength(50, ErrorMessage = "Display Name cannot exceed 50 characters.")]
        public string? DisplayName { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        public List<string>? SkillNames { get; set; }
    }
}
