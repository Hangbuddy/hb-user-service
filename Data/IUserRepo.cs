using System.Collections.Generic;
using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();
        
        User GetUser(string username);

        void CreateUser(User user);

        void UpdateUser(User user);

        User Login(User user);
    }
}