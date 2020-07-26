using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class RemoveLoginForUserQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public RemoveLoginForUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveLoginForUserQuery
                .ReplaceQueryParameters(
                    _sqlConfiguration.SchemaName,
                    _sqlConfiguration.UserLoginTable,
                    _sqlConfiguration.ParameterNotation,
                    new[] { "%USERID%", "%LOGINPROVIDER%", "%PROVIDERKEY%", },
                    new[] { "UserId", "LoginProvider", "ProviderKey", });

            return query;
        }
    }
}
