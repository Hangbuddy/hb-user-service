using System;
using System.Linq;
using UserService.Models;

namespace UserService.Data
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public ApplicationUserRead GetUser(string userId)
        {
            var applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(p => p.Id == userId);

            return new ApplicationUserRead()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                DisplayName = applicationUser.DisplayName,
                Bio = applicationUser.Bio,
                IsActive = applicationUser.IsActive
            };
        }

        public void UpdateUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.ApplicationUsers.Update(user);
        }

        public void CreateUser(ApplicationUser user)
        {
            _context.ApplicationUsers.Add(user);
        }
    }
}