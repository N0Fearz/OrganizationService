using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OrganizationService.Data;
using OrganizationService.Models;
using OrganizationService.Services;

namespace OrganizationService.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly OrganizationContext _organizationContext;
        public OrganizationRepository(OrganizationContext organizationContext, MigrationService migrationService)
        {
            _organizationContext = organizationContext;

            migrationService.MigrateAsync().GetAwaiter().GetResult();
        }
        public Organization GetOrganizationById(Guid id)
        {
            var organization = _organizationContext.Organizations.FirstOrDefault(d => d.OrgId == id);
            return organization;
        }

        public void AddOrganization(Organization organization)
        {
            _organizationContext.Organizations.Add(organization);
            _organizationContext.SaveChanges();
        }

        public void RemoveOrganization(Organization organization)
        {
            _organizationContext.Organizations.Remove(organization);
            Save();
        }

        public IEnumerable<Organization> GetOrganizations()
        {
            return _organizationContext.Organizations.ToList();
        }

        public void Save()
        {
            _organizationContext.SaveChanges();
        }
    }
}
