using System.ComponentModel.DataAnnotations;

namespace OrganizationService.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public Guid OrgId { get; set; }
        public string OrgName { get; set; }
        public string ConnectionString { get; set; }
    }
}
