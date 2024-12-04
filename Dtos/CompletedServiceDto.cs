namespace ServiceManagementAPI.Dtos
{
    public class CompletedServiceDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? MediaUrl { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public DateTime CompletedAt { get; set; }
    }
}
