using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PreferredPaymentMethod { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual AspNetUser User { get; set; } = null!;
}
