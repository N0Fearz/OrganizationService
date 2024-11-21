using Microsoft.EntityFrameworkCore;
using OrganizationService.Models;

namespace OrganizationService.Data
{
    public class OrganizationContext(DbContextOptions<OrganizationContext> options) : DbContext(options)
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
