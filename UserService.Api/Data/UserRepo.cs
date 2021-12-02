using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UserService.Dtos.Enums;
using UserService.Models;

namespace UserService.Data
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public UserRepo(AppDbContext context,
                        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public List<ApplicationUserRead> GetUsersBulk(List<string> userIdList)
        {
            return userIdList.Select(userId => GetUser(userId)).ToList();
        }

        public async Task<bool> UpdateUser(ApplicationUser user, string password)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }
                _context.ApplicationUsers.Update(user);
                IdentityUser cUser = await _userManager.FindByIdAsync(user.Id);
                var token = await _userManager.GeneratePasswordResetTokenAsync(cUser);
                IdentityResult result = await _userManager.ResetPasswordAsync(cUser, token, password);
                return result.Succeeded;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<ErrorCodes> CreateUser(ApplicationUser applicationUser, IdentityUser identityUser, string password)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (await _userManager.FindByNameAsync(identityUser.UserName) != null)
                    return ErrorCodes.UsernameAlreadyInUse;
                if (await _userManager.FindByEmailAsync(identityUser.Email) != null)
                    return ErrorCodes.EmailAlreadyInUse;

                _context.ApplicationUsers.Add(applicationUser);

                var isCreated = await _userManager.CreateAsync(identityUser, password);
                if (isCreated.Succeeded)
                    return ErrorCodes.Success;
                else
                {
                    // ToDo: Remove application user
                    // ToDo: Log errors.
                    var errors = isCreated.Errors.Select(x => x.Description).ToList();
                    return ErrorCodes.UnknownError;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return ErrorCodes.UnknownError;
            }
        }
    }
}