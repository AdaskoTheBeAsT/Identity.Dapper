using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Identity.Dapper.Entities;

namespace Identity.Dapper.Repositories.Contracts
{
    public interface IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : struct, IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        Task<bool> InsertAsync(TRole role, CancellationToken cancellationToken);

        Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(TRole role, CancellationToken cancellationToken);

        Task<TRole> GetByIdAsync(TKey id, CancellationToken cancellationToken);

        Task<TRole> GetByNameAsync(string roleName, CancellationToken cancellationToken);

        Task<IEnumerable<TRoleClaim>> GetClaimsByRoleAsync(TRole role, CancellationToken cancellationToken);

        Task<bool> InsertClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken);

        Task<bool> RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken);
    }
}
