using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Identity.Dapper.Samples.Web
{
    public static class Program
    {
        public static void Main()
        {
            using var host = new WebHostBuilder()
                          .UseKestrel()
                          .UseContentRoot(Directory.GetCurrentDirectory())
                          .UseIISIntegration()
                          .UseStartup<Startup>()
                          .Build();

            host.Run();
        }
    }
}
