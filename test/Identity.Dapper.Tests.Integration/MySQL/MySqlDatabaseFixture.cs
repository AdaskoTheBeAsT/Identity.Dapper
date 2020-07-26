using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Identity.Dapper.Tests.Integration.MySQL
{
    public sealed class MySqlDatabaseFixture
        : IDisposable
    {
        public MySqlDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupMySql>();
            TestServer = new TestServer(builder);
        }

        public TestServer TestServer { get; }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
