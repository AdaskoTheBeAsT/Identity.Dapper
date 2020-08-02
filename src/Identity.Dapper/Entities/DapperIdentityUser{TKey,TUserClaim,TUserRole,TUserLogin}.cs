using System;
using System.Collections.Generic;

namespace Identity.Dapper.Entities
{
#pragma warning disable MA0048 // File name must match type name
    public class DapperIdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
#pragma warning restore MA0048 // File name must match type name
        where TKey : struct, IEquatable<TKey>
    {
        public DapperIdentityUser()
        {
            UserName = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            SecurityStamp = string.Empty;
            PhoneNumber = string.Empty;
        }

        public DapperIdentityUser(string userName)
            : this()
        {
            UserName = userName;
            Email = string.Empty;
            PasswordHash = string.Empty;
            SecurityStamp = string.Empty;
            PhoneNumber = string.Empty;
        }

        public TKey Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public ICollection<TUserRole> Roles { get; } = new List<TUserRole>();

        public ICollection<TUserClaim> Claims { get; } = new List<TUserClaim>();

        public ICollection<TUserLogin> Logins { get; } = new List<TUserLogin>();

        public override string ToString()
        {
            return UserName;
        }
    }
}
