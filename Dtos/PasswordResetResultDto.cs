namespace ServiceManagementAPI.Dtos
{
    public class PasswordResetResultDto
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
