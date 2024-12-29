using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OrganizationService.Data;
using OrganizationService.Models;
using OrganizationService.Repository;

namespace OrganizationService.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly MigrationService _migrationService;
    public OrganizationService(IOrganizationRepository organizationRepository, MigrationService migrationService)
    {
        _organizationRepository = organizationRepository;
        _migrationService = migrationService;
        
    }
    public string AddOrganization(string organizationMessage)
    {
        if (organizationMessage != null)
        {
            if (!JObject.Parse(organizationMessage).ContainsKey("name"))
            {
                Console.WriteLine("Error: Organization Name is missing");
                throw new ArgumentNullException(organizationMessage, "Organization Name is missing");
            }

            if (!JObject.Parse(organizationMessage).ContainsKey("id"))
            {
                Console.WriteLine("Error: Organization Id is missing");
                throw new ArgumentNullException(organizationMessage, "Organization Id is missing");
            }
            
            var orgId = (Guid)JObject.Parse(organizationMessage)["id"]!;
            var orgName = (string)JObject.Parse(organizationMessage)["name"]!;
            var schemaName = AddOrganizationDbSchema(orgName);
        
            Organization organization = new Organization
            {
                OrgId = orgId,
                OrgName = orgName,
                SchemaName = schemaName.Result
            };
            _organizationRepository.AddOrganization(organization);
            return schemaName.Result;
        }
        
        throw new ArgumentNullException(organizationMessage, "organization message is missing");
    }

    private async Task Migrate()
    {
        await _migrationService.MigrateAsync();
    }
    
    public string CheckOrganization(Guid organizationId)
    {
        var result = _organizationRepository.GetOrganizationById(organizationId);
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), "Organization not found.");
        }
        if (result.OrgName == null)
        {
            throw new ArgumentNullException(nameof(result.OrgName), "Organization name is null.");
        }

        return result.SchemaName;
    }

    public Organization GetOrganization(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public void DeleteOrganization(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> AddOrganizationDbSchema(string orgName)
    {
        await Migrate();
        var schemaName = $"schema_{orgName.ToLower()}";
        _organizationRepository.AddOrganizationDbSchema(schemaName);
        
        return schemaName;
    }
}