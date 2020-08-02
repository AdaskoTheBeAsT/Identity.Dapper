using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Identity.Dapper.Samples.Web
{
    public static class Program
    {
#pragma warning disable CA1031 // Do not catch general exception types
        public static int Main(string[] args)
        {
            var host = default(IWebHost);
            try
            {
                host = CreateWebHostBuilder(args).Build();
                host.Run();
                return 0;
            }
#pragma warning disable CC0003 // Your catch should include an Exception
            catch
            {
                return 1;
            }
#pragma warning restore CC0003 // Your catch should include an Exception
            finally
            {
                host?.Dispose();
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)

                // .ConfigureLogging(logging => logging.ClearProviders())
                .UseStartup<Startup>()
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .ConfigureAppConfiguration(
                    (
                        hostContext,
                        builder) =>
                    {
                        builder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile(
                                $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                                optional: true);
                        if (hostContext.HostingEnvironment.IsDevelopment())
                        {
                            builder.AddUserSecrets<Startup>();
                        }

                        builder.AddEnvironmentVariables();
                    });
    }
}
