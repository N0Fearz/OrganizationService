using OrganizationService.Data;
using OrganizationService.Models;

namespace OrganizationService.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly OrganizationContext _organizationContext;
        public OrganizationRepository(OrganizationContext organizationContext)
        {
            _organizationContext = organizationContext;
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
