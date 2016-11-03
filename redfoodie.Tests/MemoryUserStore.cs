using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using redfoodie.Models;

namespace redfoodie.Tests
{
    public class MemoryUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IQueryableUserStore<TUser, string>, IUserEmailStore<TUser> where TUser : class, IUser<string>
    {
        private readonly Dictionary<string, TUser> _users = new Dictionary<string, TUser>();

        public IQueryable<TUser> Users => _users.Values.AsQueryable();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(Users.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.CurrentCultureIgnoreCase)));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            var applicationUser = user as ApplicationUser;
            if (applicationUser == null)
                throw new ArgumentNullException(nameof(user));
            applicationUser.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            var applicationUser = user as ApplicationUser;
            if (applicationUser == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(applicationUser.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(u => string.Equals((u as ApplicationUser).Email, email, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}