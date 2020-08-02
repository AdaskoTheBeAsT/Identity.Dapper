using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Models;
using Identity.Dapper.Queries.Role;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Repositories
{
    public class RoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : struct, IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryFactory _queryFactory;

        public RoleRepository(IConnectionProvider connectionProvider, ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> log, SqlConfiguration sqlConfiguration, IUnitOfWork unitOfWork, IQueryFactory queryFactory)
        {
            _connectionProvider = connectionProvider;
            _log = log;
            _sqlConfiguration = sqlConfiguration;
            _unitOfWork = unitOfWork;
            _queryFactory = queryFactory;
        }

        public async Task<TRole> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _queryFactory.GetQuery<SelectRoleByIdQuery>();

                    return await x.QueryFirstOrDefaultAsync<TRole>(
                        sql: query,
                        param: dynamicParameters,
                        transaction: _unitOfWork.Transaction).ConfigureAwait(false);
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await selectFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<TRole> GetByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Name", roleName);

                    var query = _queryFactory.GetQuery<SelectRoleByNameQuery>();

                    return await x.QueryFirstOrDefaultAsync<TRole>(
                        sql: query,
                        param: dynamicParameters,
                        transaction: _unitOfWork.Transaction)
                        .ConfigureAwait(false);
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await selectFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();

                    return await selectFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public Task<IEnumerable<TRoleClaim>> GetClaimsByRoleAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return GetClaimsByInternalRoleAsync(role, cancellationToken);
        }

        public async Task<bool> InsertAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters(role);

                    var query = _queryFactory.GetInsertQuery<InsertRoleQuery, TRole>(role);

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction).ConfigureAwait(false);

                    return result > 0;
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await insertFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await insertFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public Task<bool> InsertClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return InsertClaimInternalAsync(role, claim, cancellationToken);
        }

        public async Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _queryFactory.GetDeleteQuery<DeleteRoleQuery>();

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction).ConfigureAwait(false);

                    return result > 0;
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await removeFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await removeFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public Task<bool> RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return RemoveClaimInternalAsync(role, claim, cancellationToken);
        }

        public Task<bool> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return UpdateInternalAsync(role, cancellationToken);
        }

        private async Task<IEnumerable<TRoleClaim>> GetClaimsByInternalRoleAsync(
            TRole role,
            CancellationToken cancellationToken)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IEnumerable<TRoleClaim>>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("RoleId", role.Id);

                    var query = _queryFactory.GetQuery<GetClaimsByRoleQuery>();

                    return await x.QueryAsync<TRoleClaim>(
                            sql: query,
                            param: dynamicParameters,
                            transaction: _unitOfWork.Transaction)
                        .ConfigureAwait(false);
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await selectFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        private async Task<bool> InsertClaimInternalAsync(
            TRole role,
            Claim claim,
            CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var roleClaim = Activator.CreateInstance<TRoleClaim>();
                    roleClaim.ClaimType = claim.Type;
                    roleClaim.ClaimValue = claim.Value;
                    roleClaim.RoleId = role.Id;

                    var dynamicParameters = new DynamicParameters(roleClaim);

                    var query = _queryFactory.GetInsertQuery<InsertRoleClaimQuery, TRoleClaim>(roleClaim);

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction).ConfigureAwait(false);

                    return result > 0;
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await insertFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await insertFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        private async Task<bool> RemoveClaimInternalAsync(
            TRole role,
            Claim claim,
            CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("RoleId", role.Id);
                    dynamicParameters.Add("ClaimType", claim.Type);
                    dynamicParameters.Add("ClaimValue", claim.Value);

                    var query = _queryFactory.GetDeleteQuery<DeleteRoleClaimQuery>();

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction).ConfigureAwait(false);

                    return result > 0;
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return await removeFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await removeFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        private async Task<bool> UpdateInternalAsync(
            TRole role,
            CancellationToken cancellationToken)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters(role);

                    var query = _queryFactory.GetUpdateQuery<UpdateRoleQuery, TRole>(role);

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction).ConfigureAwait(false);

                    return result > 0;
                });

                if (_unitOfWork?.Connection == null)
                {
                    using var conn = _connectionProvider.Create();
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
                    return await updateFunction(conn).ConfigureAwait(false);
                }
                else
                {
                    var conn = _unitOfWork.CreateOrGetConnection();
                    return await updateFunction(conn).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}
