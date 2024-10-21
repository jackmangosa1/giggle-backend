namespace ServiceManagementAPI.Entities;

public partial class Service
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? MediaUrl { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ServiceCategory Category { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;
}
