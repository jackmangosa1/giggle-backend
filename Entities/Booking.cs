using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public int BookingStatus { get; set; }

    public int PaymentStatus { get; set; }

    public decimal PaymentAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? EscrowAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public virtual ICollection<CompletedService> CompletedServices { get; set; } = new List<CompletedService>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Service Service { get; set; } = null!;
}
