using System;
using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class GetUsersInRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUsersInRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: true, ignoreProperties: new string[] { "ConcurrencyStamp" });

#pragma warning disable SA1118 // Parameter should not span multiple lines
            var query = _sqlConfiguration.GetUsersInRoleQuery
                .ReplaceQueryParameters(
                    _sqlConfiguration.SchemaName,
                    string.Empty,
                    _sqlConfiguration.ParameterNotation,
                    new[] { "%ROLENAME%", },
                    new[] { "RoleName", },
                    new[] { "%USERFILTER%", "%USERTABLE%", "%USERROLETABLE%", "%ROLETABLE%", },
                    new[]
                    {
                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                        _sqlConfiguration.UserTable,
                        _sqlConfiguration.UserRoleTable,
                        _sqlConfiguration.RoleTable,
                    });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            return query;
        }
    }
}
