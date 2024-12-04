namespace ServiceManagementAPI.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = null!;
    }
}
