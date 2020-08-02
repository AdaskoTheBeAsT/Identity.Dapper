using System;

namespace Identity.Dapper.Models
{
    public abstract class SqlConfiguration
    {
        protected SqlConfiguration()
        {
            SchemaName = string.Empty;
            ParameterNotation = string.Empty;
            TableColumnStartNotation = string.Empty;
            TableColumnEndNotation = string.Empty;
            RoleTable = string.Empty;
            UserTable = string.Empty;
            UserClaimTable = string.Empty;
            UserLoginTable = string.Empty;
            UserRoleTable = string.Empty;
            RoleClaimTable = string.Empty;
            InsertRoleQuery = string.Empty;
            DeleteRoleQuery = string.Empty;
            UpdateRoleQuery = string.Empty;
            SelectRoleByNameQuery = string.Empty;
            SelectRoleByIdQuery = string.Empty;
            SelectClaimByRoleQuery = string.Empty;
            InsertRoleClaimQuery = string.Empty;
            DeleteRoleClaimQuery = string.Empty;
            InsertUserQuery = string.Empty;
            DeleteUserQuery = string.Empty;
            UpdateUserQuery = string.Empty;
            SelectUserByUserNameQuery = string.Empty;
            SelectUserByEmailQuery = string.Empty;
            SelectUserByIdQuery = string.Empty;
            InsertUserClaimQuery = string.Empty;
            InsertUserLoginQuery = string.Empty;
            InsertUserRoleQuery = string.Empty;
            GetUserLoginByLoginProviderAndProviderKeyQuery = string.Empty;
            GetClaimsByUserIdQuery = string.Empty;
            GetUserLoginInfoByIdQuery = string.Empty;
            GetUsersByClaimQuery = string.Empty;
            GetUsersInRoleQuery = string.Empty;
            GetRolesByUserIdQuery = string.Empty;
            IsInRoleQuery = string.Empty;
            RemoveClaimsQuery = string.Empty;
            RemoveUserFromRoleQuery = string.Empty;
            RemoveLoginForUserQuery = string.Empty;
            UpdateClaimForUserQuery = string.Empty;
        }

        public string SchemaName { get; set; }

        public string ParameterNotation { get; set; }

        public string TableColumnStartNotation { get; set; }

        public string TableColumnEndNotation { get; set; }

        // Table Names
        public string RoleTable { get; set; }

        public string UserTable { get; set; }

        public string UserClaimTable { get; set; }

        public string UserLoginTable { get; set; }

        public string UserRoleTable { get; set; }

        public string RoleClaimTable { get; set; }

        // Role Queries
        public string InsertRoleQuery { get; set; }

        public string DeleteRoleQuery { get; set; }

        public string UpdateRoleQuery { get; set; }

        public string SelectRoleByNameQuery { get; set; }

        public string SelectRoleByIdQuery { get; set; }

        public string SelectClaimByRoleQuery { get; set; }

        public string InsertRoleClaimQuery { get; set; }

        public string DeleteRoleClaimQuery { get; set; }

        // User Queries
        public string InsertUserQuery { get; set; }

        public string DeleteUserQuery { get; set; }

        public string UpdateUserQuery { get; set; }

        public string SelectUserByUserNameQuery { get; set; }

        public string SelectUserByEmailQuery { get; set; }

        public string SelectUserByIdQuery { get; set; }

        public string InsertUserClaimQuery { get; set; }

        public string InsertUserLoginQuery { get; set; }

        public string InsertUserRoleQuery { get; set; }

        public string GetUserLoginByLoginProviderAndProviderKeyQuery { get; set; }

        public string GetClaimsByUserIdQuery { get; set; }

        public string GetUserLoginInfoByIdQuery { get; set; }

        public string GetUsersByClaimQuery { get; set; }

        public string GetUsersInRoleQuery { get; set; }

        public string GetRolesByUserIdQuery { get; set; }

        public string IsInRoleQuery { get; set; }

        public string RemoveClaimsQuery { get; set; }

        public string RemoveUserFromRoleQuery { get; set; }

        public string RemoveLoginForUserQuery { get; set; }

        public string UpdateClaimForUserQuery { get; set; }
    }
}
