using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Provider
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? Bio { get; set; }

    public string? DisplayName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
