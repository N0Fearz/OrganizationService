using OrganizationService.Models;

namespace OrganizationService.Repository
{
    public interface IOrganizationRepository
    {
        IEnumerable<Organization> GetOrganizations();
        Organization GetOrganizationById(Guid id);
        void Save();
    }
}
