using System.ComponentModel.DataAnnotations;

namespace OrganizationService.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
