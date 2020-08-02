using System;
using Identity.Dapper.Models;
using Identity.Dapper.Queries.Contracts;

namespace Identity.Dapper.Queries.User
{
    public class GetUsersByClaimQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUsersByClaimQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(
                _sqlConfiguration,
                ignoreIdProperty: true,
                ignoreProperties: new string[] { "ConcurrencyStamp" });

#pragma warning disable SA1118 // Parameter should not span multiple lines
            var query = _sqlConfiguration.GetUsersByClaimQuery
                .ReplaceQueryParameters(
                    _sqlConfiguration.SchemaName,
                    _sqlConfiguration.UserTable,
                    _sqlConfiguration.ParameterNotation,
                    new[] { "%CLAIMVALUE%", "%CLAIMTYPE%", },
                    new[] { "ClaimValue", "ClaimType", },
                    new[] { "%USERFILTER%", "%USERTABLE%", "%USERCLAIMTABLE%", },
                    new[]
                    {
                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                        _sqlConfiguration.UserTable,
                        _sqlConfiguration.UserClaimTable,
                    });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            return query;
        }
    }
}
