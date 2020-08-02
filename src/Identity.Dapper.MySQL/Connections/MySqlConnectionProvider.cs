using System;
using System.Data.Common;
using Identity.Dapper.Connections;
using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Identity.Dapper.MySQL.Exceptions;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Identity.Dapper.MySQL.Connections
{
    public class MySqlConnectionProvider : IConnectionProvider
    {
        private readonly IOptions<ConnectionProviderOptions> _connectionProviderOptions;
        private readonly EncryptionHelper _encryptionHelper;

        public MySqlConnectionProvider(IOptions<ConnectionProviderOptions> connProvOpts, EncryptionHelper encHelper)
        {
            _connectionProviderOptions = connProvOpts ?? throw new ArgumentNullException(nameof(connProvOpts));
            _encryptionHelper = encHelper ?? throw new ArgumentNullException(nameof(encHelper));
        }

        public DbConnection Create()
        {
            if (_connectionProviderOptions.Value is null)
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
                var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
                {
                    Password = _encryptionHelper.TryDecryptAES256(password ?? string.Empty),
                    UserID = username,
                };
                connectionString = connectionStringBuilder.ToString();
            }

            return new MySqlConnection(connectionString);
        }
    }
}
