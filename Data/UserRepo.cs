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

        public ApplicationUser GetUser(string userId)
        {
            return _context.ApplicationUsers.FirstOrDefault(p => p.Id == userId);
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