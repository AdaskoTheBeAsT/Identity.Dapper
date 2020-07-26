namespace Identity.Dapper.Entities
{
    public class DapperIdentityRole : DapperIdentityRole<int>
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
