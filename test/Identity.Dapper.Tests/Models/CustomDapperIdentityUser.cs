using Identity.Dapper.Entities;

namespace Identity.Dapper.Tests.Models
{
    public class CustomDapperIdentityUser : DapperIdentityUser
    {
        public string? Dummy { get; set; }
    }
}
