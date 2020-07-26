using Identity.Dapper.Entities;

namespace Identity.Dapper.Samples.Web.Entities
{
    public class CustomUser : DapperIdentityUser
    {
        public CustomUser()
        {
        }

        public CustomUser(string userName)
            : base(userName)
        {
        }

        public string? Address { get; set; }
    }
}
