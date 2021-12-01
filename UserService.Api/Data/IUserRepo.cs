using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserService.Dtos.Enums;
using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        ApplicationUserRead GetUser(string userId);

        Task<bool> UpdateUser(ApplicationUser user, string password);

        Task<ErrorCodes> CreateUser(ApplicationUser applicationUser, IdentityUser identityUser, string password);
    }
}