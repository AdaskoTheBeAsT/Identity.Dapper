using System;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.SqlServer.Connections;
using Identity.Dapper.SqlServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Tests.Integration.SQLServer
{
    public class TestStartupSqlServer
    {
        public TestStartupSqlServer(IWebHostEnvironment env)
        {
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("integrationtest.config.sqlserver.json", optional: false, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<TestStartupSqlServer>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(
                options =>
                {
                    options.AddConsole();
                    options.AddDebug();
                });

            services.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(Configuration.GetSection("DapperIdentity"))
                    .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
                    .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); // Change to True to use Transactions in all operations

            services.AddIdentity<DapperIdentityUser, DapperIdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                    .AddDapperIdentityFor<SqlServerConfiguration>()
                    .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, FakeAuthMessageSender>();
            services.AddTransient<ISmsSender, FakeAuthMessageSender>();
        }
    }
}
