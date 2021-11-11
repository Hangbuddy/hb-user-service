using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        ApplicationUser GetUser(string userId);

        void UpdateUser(ApplicationUser user);

        void CreateUser(ApplicationUser user);
    }
}