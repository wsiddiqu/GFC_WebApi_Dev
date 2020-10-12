using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GFC.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string localIp = "*";
                    string serverManagerPort = "8999";
                    string baseUrl = string.Format("https://{0}:{1}", localIp, serverManagerPort);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:8998",baseUrl);
                }).UseWindowsService();
    }
}
