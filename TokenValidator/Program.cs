using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace TokenValidator
{
    public class Program
    {

        public static void Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;
            TokenValidationParamsStore.Start();
            CreateHostBuilder(args).Build().Run();
            TokenValidationParamsStore.Stop();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
