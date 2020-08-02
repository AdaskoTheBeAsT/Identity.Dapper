using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class SelectUserByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public SelectUserByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
#pragma warning disable SA1118 // Parameter should not span multiple lines
            var query = _sqlConfiguration.SelectUserByIdQuery
                .ReplaceQueryParameters(
                    _sqlConfiguration.SchemaName,
                    string.Empty,
                    _sqlConfiguration.ParameterNotation,
                    new[] { "%ID%", },
                    new[] { "Id", },
                    new[] { "%USERTABLE%", "%ROLETABLE%", "%USERROLETABLE%", },
                    new[]
                    {
                        _sqlConfiguration.UserTable, _sqlConfiguration.RoleTable, _sqlConfiguration.UserRoleTable,
                    });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
