﻿using System;
using System.Collections.Generic;

namespace ServiceManagementAPI.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
