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
            throw new NotImplementedException();
        }

        public IEnumerable<Organization> GetOrganizations()
        {
            return _organizationContext.Organizations.ToList();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
