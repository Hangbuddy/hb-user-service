using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return (_context.SaveChanges() >= 0);
        }

        public User GetUser(int userId)
        {
            return _context.Users.FirstOrDefault(p => p.Id == userId);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Update(user);
        }

        public User Login(User user)
        {
            var _user = _context.Users.FirstOrDefault(p => p.Username == user.Username);
            if (_user is null)
                return null;
            if (_user.Password.Equals(user.Password))
                return _user;
            else
                return null;
        }

    }
}