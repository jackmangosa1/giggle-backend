using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
}
