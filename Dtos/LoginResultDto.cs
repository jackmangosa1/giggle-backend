namespace ServiceManagementAPI.Dtos
{
    public class LoginResultDto
    {
        public bool Succeeded { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool IsLockedOut { get; set; }
        public bool RequiresEmailConfirmation { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
