namespace ServiceManagementAPI.Dtos
{
    public class ForgotPasswordtResultDto
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string>? Message { get; set; }
    }
}
