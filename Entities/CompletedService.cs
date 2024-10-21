namespace ServiceManagementAPI.Entities;

public partial class CompletedService
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string? Description { get; set; }

    public string? MediaUrl { get; set; }

    public DateTime CompletedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
