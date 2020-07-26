namespace Identity.Dapper.Entities
{
    public class DapperIdentityUser
        : DapperIdentityUser<int>
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
