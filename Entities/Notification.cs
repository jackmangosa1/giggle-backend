using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? BookingStatus { get; set; }

    public int? BookingId { get; set; }

    public string? CustomerName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
