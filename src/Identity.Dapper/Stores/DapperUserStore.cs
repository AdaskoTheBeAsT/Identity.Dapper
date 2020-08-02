using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Stores
{
    public class DapperUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>
        where TUser : DapperIdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TKey : struct, IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
        where TUserClaim : DapperIdentityUserClaim<TKey>
        where TUserLogin : DapperIdentityUserLogin<TKey>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<DapperUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> _log;
        private readonly IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> _userRepository;
        private readonly DapperIdentityOptions _dapperIdentityOptions;

        public DapperUserStore(
            IConnectionProvider connProv,
            ILogger<DapperUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> log,
            IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> roleRepo,
            IUnitOfWork uow,
            DapperIdentityOptions dapperIdOpts)
        {
            _userRepository = roleRepo;
            _connectionProvider = connProv;
            _log = log;
            _unitOfWork = uow;
            _dapperIdentityOptions = dapperIdOpts;
        }

        public IQueryable<TUser> Users
        {
            get
            {
                // Impossible to implement IQueryable with Dapper
                throw new NotSupportedException();
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            !_dapperIdentityOptions.UseTransactionalBehavior
                        ? Task.CompletedTask
                        : CommitTransactionAsync(cancellationToken);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return AddClaimsInternalAsync(user, claims, cancellationToken);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return AddLoginInternalAsync(user, login, cancellationToken);
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return AddToRoleInternalAsync(user, roleName, cancellationToken);
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return CreateInternalAsync(user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return DeleteInternalAsync(user, cancellationToken);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            return FindByEmailInternalAsync(normalizedEmail, cancellationToken);
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return FindByIdInternalAsync(userId, cancellationToken);
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await _userRepository.GetByUserLoginAsync(loginProvider, providerKey, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            return FindByNameInternalAsync(normalizedUserName, cancellationToken);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return GetClaimsInternalAsync(user, cancellationToken);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email ?? string.Empty);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnd);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return GetLoginsInternalAsync(user, cancellationToken);
        }

        #pragma warning disable S4144 // Methods should not have identical implementations
        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email ?? string.Empty);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName ?? string.Empty);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash ?? string.Empty);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumber ?? string.Empty);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return GetRolesInternalAsync(user, cancellationToken);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp ?? string.Empty);
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

#pragma warning disable S4144 // Methods should not have identical implementations
        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName ?? string.Empty);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return GetUsersForClaimInternalAsync(claim, cancellationToken);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return GetUsersInRoleInternalAsync(roleName, cancellationToken);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return IsInRoleInternalAsync(user, roleName, cancellationToken);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            return RemoveClaimsInternalAsync(user, claims, cancellationToken);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return RemoveFromRoleInternalAsync(user, roleName, cancellationToken);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            return RemoveLoginInternalAsync(user, loginProvider, providerKey, cancellationToken);
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            return ReplaceClaimInternalAsync(user, claim, newClaim, cancellationToken);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd;

            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return UpdateInternalAsync(user, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#pragma warning disable S1066 // Collapsible "if" statements should be merged
                if (_dapperIdentityOptions.UseTransactionalBehavior)
#pragma warning restore S1066 // Collapsible "if" statements should be merged
                {
#pragma warning disable IDISP007 // Don't dispose injected.
                    _unitOfWork?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                }
            }
        }

        private async Task AddClaimsInternalAsync(
            TUser user,
            IEnumerable<Claim> claims,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.InsertClaimsAsync(user.Id, claims, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task AddLoginInternalAsync(
            TUser user,
            UserLoginInfo login,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.InsertLoginInfoAsync(user.Id, login, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task AddToRoleInternalAsync(
            TUser user,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.AddToRoleAsync(user.Id, roleName, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task<IdentityResult> CreateInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.InsertAsync(user, cancellationToken).ConfigureAwait(false);

                if (!result.Equals(default))
                {
                    user.Id = result;

                    return IdentityResult.Success;
                }

                return IdentityResult.Failed();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError { Description = ex.Message },
                });
            }
        }

        private async Task<IdentityResult> DeleteInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.RemoveAsync(user.Id, cancellationToken).ConfigureAwait(false);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError { Description = ex.Message },
                });
            }
        }

        private async Task<TUser> FindByEmailInternalAsync(
            string normalizedEmail,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken).ConfigureAwait(false);

                return result;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task<TUser> FindByIdInternalAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            try
            {
                TKey key;

                var converter = TypeDescriptor.GetConverter(typeof(TKey));
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    key = (TKey)converter.ConvertFromInvariantString(userId);
                }
                else
                {
                    key = (TKey)Convert.ChangeType(userId, typeof(TKey), CultureInfo.InvariantCulture);
                }

                return await _userRepository.GetByIdAsync(key, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<TUser> FindByNameInternalAsync(
            string normalizedUserName,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.GetByUserNameAsync(normalizedUserName, cancellationToken).ConfigureAwait(false);

                return result;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<IList<Claim>> GetClaimsInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.GetClaimsByUserIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<IList<UserLoginInfo>> GetLoginsInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.GetUserLoginInfoByIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<IList<string>> GetRolesInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.GetRolesByUserIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<IList<TUser>> GetUsersForClaimInternalAsync(
            Claim claim,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.GetUsersByClaimAsync(claim, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task<IList<TUser>> GetUsersInRoleInternalAsync(
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.GetUsersInRoleAsync(roleName, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        private async Task<bool> IsInRoleInternalAsync(
            TUser user,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _userRepository.IsInRoleAsync(user.Id, roleName, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                return false;
            }
        }

        private async Task RemoveClaimsInternalAsync(
            TUser user,
            IEnumerable<Claim> claims,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.RemoveClaimsAsync(user.Id, claims, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
            }
        }

        private async Task RemoveFromRoleInternalAsync(
            TUser user,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.RemoveFromRoleAsync(user.Id, roleName, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
            }
        }

        private async Task RemoveLoginInternalAsync(
            TUser user,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.RemoveLoginAsync(user.Id, loginProvider, providerKey, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
            }
        }

        private async Task ReplaceClaimInternalAsync(
            TUser user,
            Claim claim,
            Claim newClaim,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.UpdateClaimAsync(user.Id, claim, newClaim, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
            }
        }

        private async Task<IdentityResult> UpdateInternalAsync(
            TUser user,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError { Description = ex.Message },
                });
            }
        }

        private Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (_dapperIdentityOptions.UseTransactionalBehavior)
            {
                try
                {
                    _unitOfWork.CommitChanges();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    _log.LogError(ex.Message, ex);

                    _unitOfWork.DiscardChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}
