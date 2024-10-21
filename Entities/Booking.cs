namespace ServiceManagementAPI.Entities;

public partial class Booking
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public int CustomerId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ScheduledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<CompletedService> CompletedServices { get; set; } = new List<CompletedService>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Service Service { get; set; } = null!;
}
