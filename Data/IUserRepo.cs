using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        User GetUser(int userId);

        void CreateUser(User user);

        void UpdateUser(User user);

        User Login(User user);
    }
}