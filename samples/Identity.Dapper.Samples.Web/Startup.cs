using System;
using Identity.Dapper.Models;
using Identity.Dapper.PostgreSQL.Connections;
using Identity.Dapper.PostgreSQL.Models;
using Identity.Dapper.Samples.Web.Entities;
using Identity.Dapper.Samples.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Dapper.Samples.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
            //        .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            // services.ConfigureDapperConnectionProvider<MySqlConnectionProvider>(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
            //        .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            // services.ConfigureDapperConnectionProvider<PostgreSqlConnectionProvider>(Configuration.GetSection("ConnectionStrings"))
            services.ConfigureDapperConnectionProvider<PostgreSqlConnectionProvider>(Configuration.GetSection("DapperIdentity"))
                    .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
                    .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); // Change to True to use Transactions in all operations

            services.AddIdentity<CustomUser, CustomRole>(x =>
                                                         {
                                                             x.Password.RequireDigit = false;
                                                             x.Password.RequiredLength = 1;
                                                             x.Password.RequireLowercase = false;
                                                             x.Password.RequireNonAlphanumeric = false;
                                                             x.Password.RequireUppercase = false;
                                                         })
                    .AddDapperIdentityFor<PostgreSqlConfiguration>()

                    // .AddDapperIdentityFor<SqlServerConfiguration>()
                    // .AddDapperIdentityFor<MySqlConfiguration>()
                    .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CC0091 // Use static method
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CC0091 // Use static method
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
