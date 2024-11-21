using OrganizationService.Models;

namespace OrganizationService.Repository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUserById(Guid id);
        void Save();
    }
}
