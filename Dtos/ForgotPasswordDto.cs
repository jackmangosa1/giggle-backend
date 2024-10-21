using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client URI is required.")]
        public string ClientUri { get; set; } = string.Empty;
    }
}
