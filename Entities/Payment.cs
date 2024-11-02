using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public int Method { get; set; }

    public int Status { get; set; }

    public string TransactionId { get; set; } = null!;

    public string TxRef { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? FailureReason { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
