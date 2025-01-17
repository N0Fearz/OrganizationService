﻿using OrganizationService.Models;

namespace OrganizationService.Repository
{
    public interface IOrganizationRepository
    {
        IEnumerable<Organization> GetOrganizations();
        Organization GetOrganizationById(Guid id);
        void AddOrganization(Organization organization);
        void RemoveOrganization(Organization organization);
        void Save();
    }
}
