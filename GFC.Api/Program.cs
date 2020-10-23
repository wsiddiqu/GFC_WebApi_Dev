using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GFC.Api.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GFC.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            CreateHostBuilder(args).Build().Run();
            //SeriLoggerService.Instance.Init();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string localIp = "*";
                    string serverManagerPort = "8999";
                    string baseUrl = string.Format("https://{0}:{1}", localIp, serverManagerPort);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:8998", baseUrl);
                }).UseWindowsService();
    }
}
