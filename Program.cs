using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using System.Net;

namespace netbu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("global.json")
                .Build();

            AppConfig = config;
            isPostgres = (AppConfig["isP"] == "postgres");
            string proxy = AppConfig["proxy"];
            string proxyport = AppConfig["proxyport"];
            string proxylogin = AppConfig["proxylogin"];
            string proxypassword = AppConfig["proxypassword"];
            if (!string.IsNullOrEmpty(AppConfig["proxy"]))
            {
                WebProxy wp = new WebProxy(proxy, int.Parse(proxyport));
                if (string.IsNullOrEmpty(proxylogin))
                    wp.UseDefaultCredentials = true;
                else
                    wp.Credentials = new NetworkCredential(proxylogin, proxypassword);
                WebRequest.DefaultWebProxy = wp;
                //GlobalProxySelection.Select = wp;
            }
            //Мапим диск
            try
            {
                System.Diagnostics.Process batch = new System.Diagnostics.Process();
                batch.StartInfo.FileName = @"wwwroot\Run\disk.cmd";
                batch.Start();

                timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: null,
                dueTime: 60000,
                period: 86400000
                );
            }
            catch
            {; }




            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>();
        }

        private static Timer timer;
        private static void TimerTask(object timerState)
        {
            try
            {
                System.Diagnostics.Process batch = new System.Diagnostics.Process();
                batch.StartInfo.FileName = @"C:\Projects2018\task.cmd";
                batch.Start();
            }
            catch
            {; }
        }

        public static IConfiguration AppConfig { get; set; }

        public static bool isPostgres { get; set; }
        public static bool FlagDadataUpdate = false;
    }
}
