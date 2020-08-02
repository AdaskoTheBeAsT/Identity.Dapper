using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
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
    public class DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleClaimStore<TRole>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : struct, IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;
        private readonly DapperIdentityOptions _dapperIdentityOptions;

        private DbConnection? _connection;

        public DapperRoleStore(
            IConnectionProvider connProv,
            ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> log,
            IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepo,
            IUnitOfWork uow,
            DapperIdentityOptions dapperIdOpts)
        {
            _roleRepository = roleRepo;
            _log = log;
            _connectionProvider = connProv;
            _unitOfWork = uow;
            _dapperIdentityOptions = dapperIdOpts;
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

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var result = await _roleRepository.InsertAsync(role, cancellationToken).ConfigureAwait(false);

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

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var result = await _roleRepository.RemoveAsync(role.Id, cancellationToken).ConfigureAwait(false);

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

        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default;
            }

            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            try
            {
                var result = await _roleRepository.GetByIdAsync(ConvertIdFromString(roleId), cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                throw;
            }
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            try
            {
                var result = await _roleRepository.GetByNameAsync(normalizedRoleName, cancellationToken).ConfigureAwait(false);

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

        public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return role.Name ?? string.Empty;
        }

        public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (role.Id.Equals(default))
            {
                return string.Empty;
            }

            return role.Id.ToString();
        }

#pragma warning disable S4144 // Methods should not have identical implementations
        public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return role.Name ?? string.Empty;
        }

        public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = normalizedName;
        }

        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var result = await _roleRepository.UpdateAsync(role, cancellationToken).ConfigureAwait(false);

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

        public async Task<IList<Claim>?> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var result = await _roleRepository.GetClaimsByRoleAsync(role, cancellationToken).ConfigureAwait(false);

                return result?.Select(roleClaim => new Claim(roleClaim.ClaimType, roleClaim.ClaimValue))
                              .ToList();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                await _roleRepository.InsertClaimAsync(role, claim, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                await _roleRepository.RemoveClaimAsync(role, claim, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _log.LogError(ex.Message, ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dapperIdentityOptions.UseTransactionalBehavior)
                {
#pragma warning disable IDISP007 // Don't dispose injected.
                    _unitOfWork?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                }

                _connection?.Dispose();
            }
        }

        private async Task CreateTransactionIfNotExistsAsync(CancellationToken cancellationToken)
        {
            if (!_dapperIdentityOptions.UseTransactionalBehavior)
            {
                _connection = _connectionProvider.Create();
                await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                _connection = _unitOfWork.CreateOrGetConnection();

                if (_connection.State == System.Data.ConnectionState.Closed)
                {
                    await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                }
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
