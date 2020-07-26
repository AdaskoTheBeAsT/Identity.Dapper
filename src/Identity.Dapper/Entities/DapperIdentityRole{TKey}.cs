using System;

namespace Identity.Dapper.Entities
{
#pragma warning disable MA0048 // File name must match type name
    public class DapperIdentityRole<TKey>
#pragma warning restore MA0048 // File name must match type name
        : DapperIdentityRole<TKey, DapperIdentityUserRole<TKey>, DapperIdentityRoleClaim<TKey>>
        where TKey : struct, IEquatable<TKey>
    {
        public DapperIdentityRole()
        {
        }

        public DapperIdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }
}
