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
    public OrganizationService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
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
                SchemaName = schemaName
            };
            _organizationRepository.AddOrganization(organization);
            return schemaName;
        }
        
        throw new ArgumentNullException(organizationMessage, "organization message is missing");
    }

    public void CheckOrganization(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public Organization GetOrganization(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public void DeleteOrganization(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public string AddOrganizationDbSchema(string orgName)
    {
        var schemaName = $"schema_{orgName.ToLower()}";
        _organizationRepository.AddOrganizationDbSchema(schemaName);
        
        return schemaName;
    }
}