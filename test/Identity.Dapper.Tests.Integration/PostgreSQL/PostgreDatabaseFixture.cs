using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    public sealed class PostgreDatabaseFixture
        : IDisposable
    {
        public PostgreDatabaseFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TestStartupPostgreSql>();
            TestServer = new TestServer(builder);
        }

        public TestServer TestServer { get; }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}
