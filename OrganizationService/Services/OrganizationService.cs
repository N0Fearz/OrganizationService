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
    private IOrganizationRepository _organizationRepository;
    private readonly ErpContext _erpContext;
    public OrganizationService(IOrganizationRepository organizationRepository, ErpContext erpContext)
    {
        _organizationRepository = organizationRepository;
        _erpContext = erpContext;
    }
    public string AddOrganization(string organizationRequest)
    {
        var orgId = (Guid)JObject.Parse(organizationRequest)["id"];
        var orgName = (string)JObject.Parse(organizationRequest)["name"];
        var schemaName = AddOrganizationDbSchema(orgName);
        var connectionString =
            $"Server=192.168.2.152;Port=5432;Database=organizations;User Id=postgres;Password=password;SearchPath={schemaName};";
        
        Organization organization = new Organization
        {
            OrgId = orgId,
            OrgName = orgName,
            ConnectionString = connectionString
        };
        _organizationRepository.AddOrganization(organization);
        return connectionString;
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
        var cmd = new StringBuilder().Append("CREATE SCHEMA IF NOT EXISTS ").Append(schemaName).ToString();
        var formattableString = FormattableStringFactory.Create(cmd);
        _erpContext.Database.ExecuteSql(formattableString);
        
        return schemaName;
    }
}