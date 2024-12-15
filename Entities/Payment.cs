using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public string TransactionId { get; set; } = null!;

    public int PaymentStatus { get; set; }

    public decimal PaymentAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? EscrowAmount { get; set; }

    public decimal? ReleasedAmount { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
