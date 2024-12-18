using OrganizationService.Models;

namespace OrganizationService.Services;

public interface IOrganizationService
{
    public string AddOrganization(string organizationMessage);
    public string CheckOrganization(Guid organizationId);
    public Organization GetOrganization(Guid organizationId);
    public void DeleteOrganization(Guid organizationId);
    public Task<string> AddOrganizationDbSchema(string orgName);
    
}