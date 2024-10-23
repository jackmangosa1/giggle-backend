using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Review
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int CompletedServiceId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CompletedService CompletedService { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
