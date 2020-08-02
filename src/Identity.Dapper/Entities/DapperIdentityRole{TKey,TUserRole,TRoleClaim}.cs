using System;
using System.Collections.Generic;

namespace Identity.Dapper.Entities
{
#pragma warning disable MA0048 // File name must match type name
    public class DapperIdentityRole<TKey, TUserRole, TRoleClaim>
#pragma warning restore MA0048 // File name must match type name
        where TKey : struct, IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        public DapperIdentityRole()
        {
        }

        public DapperIdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }

        public ICollection<TUserRole> Users { get; } = new List<TUserRole>();

        public ICollection<TRoleClaim> Claims { get; } = new List<TRoleClaim>();

        public TKey Id { get; set; }

        public string? Name { get; set; }
    }
}
