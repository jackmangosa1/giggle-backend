namespace ServiceManagementAPI.Dtos
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public string? UserId { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? Token { get; set; }
        public bool RequiresEmailConfirmation;
    }
}
