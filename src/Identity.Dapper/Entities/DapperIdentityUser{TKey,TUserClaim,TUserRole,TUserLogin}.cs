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

        public virtual TKey Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual bool LockoutEnabled { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual ICollection<TUserRole> Roles { get; } = new List<TUserRole>();

        public virtual ICollection<TUserClaim> Claims { get; } = new List<TUserClaim>();

        public virtual ICollection<TUserLogin> Logins { get; } = new List<TUserLogin>();

        public override string ToString()
        {
            return UserName;
        }
    }
}
