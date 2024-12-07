using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OrganizationService.Data;
using OrganizationService.Models;

namespace OrganizationService.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly OrganizationContext _organizationContext;
        private readonly ErpContext _erpContext;
        public OrganizationRepository(OrganizationContext organizationContext, ErpContext erpContext)
        {
            _organizationContext = organizationContext;
            _erpContext = erpContext;
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

        public void AddOrganizationDbSchema(string schemaName)
        {
            var cmd = new StringBuilder().Append("CREATE SCHEMA IF NOT EXISTS ").Append(schemaName).ToString();
            var formattableString = FormattableStringFactory.Create(cmd);
            _erpContext.Database.ExecuteSql(formattableString);
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
