using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Identity.Dapper.Tests.Integration.SQLServer
{
    public sealed class SqlServerDatabaseFixture
        : IDisposable
    {
        public SqlServerDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupSqlServer>();
            TestServer = new TestServer(builder);
        }

        public TestServer TestServer { get; }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
