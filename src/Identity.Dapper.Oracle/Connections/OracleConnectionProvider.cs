using System;
using System.Data.Common;
using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Identity.Dapper.Oracle.Exceptions;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace Identity.Dapper.Oracle.Connections
{
    public class OracleConnectionProvider
        : IConnectionProvider
    {
        private readonly IOptions<ConnectionProviderOptions> _connectionProviderOptions;
        private readonly EncryptionHelper _encryptionHelper;

        public OracleConnectionProvider(IOptions<ConnectionProviderOptions> connProvOpts, EncryptionHelper encHelper)
        {
            _connectionProviderOptions = connProvOpts ?? throw new ArgumentNullException(nameof(connProvOpts));
            _encryptionHelper = encHelper ?? throw new ArgumentNullException(nameof(encHelper));
        }

        public DbConnection Create()
        {
            if (_connectionProviderOptions.Value == null)
            {
                throw new NoDapperIdentityConfigurationSectionRegisteredException(
                    "There's no DapperIdentity configuration section registered. Please, register the section in appsettings.json or user secrets.");
            }

            var connectionOptions = _connectionProviderOptions.Value!;

            if (string.IsNullOrEmpty(connectionOptions.ConnectionString))
            {
                throw new NoDapperIdentityConnectionStringConfiguredException(
                    "There's no DapperIdentity:ConnectionString configured. Please, register the value.");
            }

            var connectionString = connectionOptions.ConnectionString!;
            var username = connectionOptions.Username;
            var password = connectionOptions.Password;

            // if both a username and password were provided, update the connection string with them
            // otherwise, leave the connection string that was configured alone
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var connectionStringBuilder = new OracleConnectionStringBuilder(connectionString)
                {
                    Password = _encryptionHelper.TryDecryptAES256(password ?? string.Empty),
                    UserID = username,
                };
                connectionString = connectionStringBuilder.ToString();
            }

            return new OracleConnection(connectionString);
        }
    }
}
