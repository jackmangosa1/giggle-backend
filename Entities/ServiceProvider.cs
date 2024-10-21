namespace ServiceManagementAPI.Entities;

public partial class ServiceProvider
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? Bio { get; set; }

    public string? Skills { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual AspNetUser User { get; set; } = null!;
}
