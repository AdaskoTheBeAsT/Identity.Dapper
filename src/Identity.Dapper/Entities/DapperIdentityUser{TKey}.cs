using System;

namespace Identity.Dapper.Entities
{
#pragma warning disable MA0048 // File name must match type name
    public class DapperIdentityUser<TKey>
#pragma warning restore MA0048 // File name must match type name
        : DapperIdentityUser<TKey, DapperIdentityUserClaim<TKey>, DapperIdentityUserRole<TKey>, DapperIdentityUserLogin<TKey>>
        where TKey : struct, IEquatable<TKey>
    {
        public DapperIdentityUser()
        {
        }

        public DapperIdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }
    }
}
