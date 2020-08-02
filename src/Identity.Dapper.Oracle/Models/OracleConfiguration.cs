using Identity.Dapper.Models;

namespace Identity.Dapper.Oracle.Models
{
    public class OracleConfiguration
        : SqlConfiguration
    {
        public OracleConfiguration()
        {
            ParameterNotation = ":";
            SchemaName = string.Empty;
            TableColumnStartNotation = string.Empty;
            TableColumnEndNotation = string.Empty;
            InsertRoleQuery = "INSERT INTO %TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleQuery = "DELETE FROM %TABLENAME% WHERE [Id] = %ID%";
            UpdateRoleQuery = "UPDATE %TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
            SelectRoleByNameQuery = "SELECT * FROM %TABLENAME% WHERE [Name] = %NAME%";
            SelectRoleByIdQuery = "SELECT * FROM %TABLENAME% WHERE [Id] = %ID%";
            InsertUserQuery = "INSERT INTO %TABLENAME% %COLUMNS% OUTPUT INSERTED.Id VALUES(%VALUES%)";
            DeleteUserQuery = "DELETE FROM %TABLENAME% WHERE [Id] = %ID%";
            UpdateUserQuery = "UPDATE %TABLENAME% %SETVALUES% WHERE [Id] = %ID%";
            SelectUserByUserNameQuery = "SELECT %USERTABLE%.*, %USERROLETABLE%.* FROM %USERTABLE% LEFT JOIN %USERROLETABLE% ON %USERROLETABLE%.[UserId] =  %USERTABLE%.[Id] WHERE [UserName] = %USERNAME%";
            SelectUserByEmailQuery = "SELECT %USERTABLE%.*, %USERROLETABLE%.* FROM %USERTABLE% LEFT JOIN %USERROLETABLE% ON %USERROLETABLE%.[UserId] =  %USERTABLE%.[Id] WHERE [Email] = %EMAIL%";
            SelectUserByIdQuery = "SELECT %USERTABLE%.*, %USERROLETABLE%.* FROM %USERTABLE% LEFT JOIN %USERROLETABLE% ON %USERROLETABLE%.[UserId] =  %USERTABLE%.[Id] WHERE [Id] = %ID%";
            InsertUserClaimQuery = "INSERT INTO %TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            InsertUserLoginQuery = "INSERT INTO %TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            InsertUserRoleQuery = "INSERT INTO %TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT TOP 1 %USERFILTER%, %USERROLETABLE%.* FROM %USERTABLE% LEFT JOIN %USERROLETABLE% ON %USERROLETABLE%.[UserId] = %USERTABLE%.[Id] INNER JOIN %USERLOGINTABLE% ON %USERTABLE%.[Id] = %USERLOGINTABLE%.[UserId] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey";
            GetClaimsByUserIdQuery = "SELECT [ClaimType], [ClaimValue] FROM %TABLENAME% WHERE [UserId] = %ID%";
            GetRolesByUserIdQuery = "SELECT [Name] FROM %ROLETABLE%, %USERROLETABLE% WHERE [UserId] = %ID% AND %ROLETABLE%.[Id] = %USERROLETABLE%.[RoleId]";
            GetUserLoginInfoByIdQuery = "SELECT [LoginProvider], [Name], [ProviderKey] FROM %TABLENAME% WHERE [UserId] = %ID%";
            GetUsersByClaimQuery = "SELECT %USERFILTER% FROM %USERTABLE%, %USERCLAIMTABLE% WHERE [ClaimValue] = %CLAIMVALUE% AND [ClaimType] = %CLAIMTYPE%";
            GetUsersInRoleQuery = "SELECT %USERFILTER% FROM %USERTABLE%, %USERROLETABLE%, %ROLETABLE% WHERE %ROLETABLE%.[Name] = %ROLENAME% AND %USERROLETABLE%.[RoleId] = %ROLETABLE%.[Id] AND %USERROLETABLE%.[UserId] = %USERTABLE%.[Id]";
            IsInRoleQuery = "SELECT 1 FROM %USERTABLE%, %USERROLETABLE%, %ROLETABLE% WHERE %ROLETABLE%.[Name] = %ROLENAME% AND %USERTABLE%.[Id] = %USERID% AND %USERROLETABLE%.[RoleId] = %ROLETABLE%.[Id] AND %USERROLETABLE%.[UserId] = %USERTABLE%.[Id]";
            RemoveClaimsQuery = "DELETE FROM %TABLENAME% WHERE [UserId] = %ID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
            RemoveUserFromRoleQuery = "DELETE FROM %USERROLETABLE% WHERE [UserId] = %USERID% AND [RoleId] = (SELECT [Id] FROM %ROLETABLE% WHERE [Name] = %ROLENAME%)";
            RemoveLoginForUserQuery = "DELETE FROM %TABLENAME% WHERE [UserId] = %USERID% AND [LoginProvider] = %LOGINPROVIDER% AND [ProviderKey] = %PROVIDERKEY%";
            UpdateClaimForUserQuery = "UPDATE %TABLENAME% SET [ClaimType] = %NEWCLAIMTYPE%, [ClaimValue] = %NEWCLAIMVALUE% WHERE [UserId] = %USERID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
            SelectClaimByRoleQuery = "SELECT %ROLECLAIMTABLE%.* FROM %ROLETABLE%, %ROLECLAIMTABLE% WHERE [RoleId] = %ROLEID% AND %ROLECLAIMTABLE%.[RoleId] = %ROLETABLE%.[Id]";
            InsertRoleClaimQuery = "INSERT INTO %TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleClaimQuery = "DELETE FROM %TABLENAME% WHERE [RoleId] = %ROLEID% AND [ClaimType] = %CLAIMTYPE% AND [ClaimValue] = %CLAIMVALUE%";
            RoleTable = "IdentityRole";
            UserTable = "IdentityUser";
            UserClaimTable = "IdentityUserClaim";
            UserRoleTable = "IdentityUserRole";
            UserLoginTable = "IdentityLogin";
            RoleClaimTable = "IdentityRoleClaim";
        }
    }
}
