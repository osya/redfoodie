using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using redfoodie.Models;

namespace redfoodie.Tests
{
    public class MemoryUserStore<TUser> : IUserLockoutStore<TUser, string>, IUserStore<TUser>, IUserPasswordStore<TUser>, IUserEmailStore<TUser>, IQueryableUserStore<TUser>, IUserTwoFactorStore<TUser, string> where TUser : class, IUser<string>
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
            return Task.FromResult(Users.FirstOrDefault(x => string.Equals((x as IdentityUser).Id, userId)));
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

        public Task<bool> HasPasswordAsync(TUser user)
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

        Task<string> IUserPasswordStore<TUser, string>.GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult((user as IdentityUser)?.PasswordHash);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var identityUser = user as IdentityUser;
            if (identityUser == null)
                throw new ArgumentNullException(nameof(identityUser));
            return Task.FromResult(identityUser.AccessFailedCount);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var identityUser = user as IdentityUser;
            return Task.FromResult(identityUser != null && identityUser.LockoutEnabled);
        }
        
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var identityUser = user as IdentityUser;
            if (identityUser == null)
                throw new ArgumentNullException(nameof(identityUser));
            return Task.FromResult(identityUser.TwoFactorEnabled);
        }
    }
}