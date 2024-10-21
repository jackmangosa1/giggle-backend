using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagementAPI.Data
{
    public class ServiceManagementIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public ServiceManagementIdentityDbContext(DbContextOptions<ServiceManagementIdentityDbContext> options)
            : base(options)
        {
        }

        protected ServiceManagementIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
