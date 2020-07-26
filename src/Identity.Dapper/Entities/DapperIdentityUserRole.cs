using System;

namespace Identity.Dapper.Entities
{
    public class DapperIdentityUserRole<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public virtual TKey UserId { get; set; }

        public virtual TKey RoleId { get; set; }
    }
}
