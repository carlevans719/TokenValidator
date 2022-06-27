using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TokenValidator
{
    public class Program
    {
        private static System.Timers.Timer aTimer;

        public static void Main(string[] args)
        {
            StartTimer();
            CreateHostBuilder(args).Build().Run();
            aTimer.Stop();
            aTimer.Dispose();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void StartTimer()
        {
            aTimer = new System.Timers.Timer(5 * 1000);
            aTimer.Elapsed += OnTimer;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimer(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Fired");
        }
    }
}
