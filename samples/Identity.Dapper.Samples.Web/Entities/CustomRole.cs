using Identity.Dapper.Entities;

namespace Identity.Dapper.Samples.Web.Entities
{
    public class CustomRole : DapperIdentityRole
    {
        public CustomRole()
        {
        }

        public CustomRole(string roleName)
            : base(roleName)
        {
        }

        public bool IsDummy { get; set; }
    }
}
