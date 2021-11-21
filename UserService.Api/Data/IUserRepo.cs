using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        ApplicationUserRead GetUser(string userId);

        void UpdateUser(ApplicationUser user);

        void CreateUser(ApplicationUser user);
    }
}