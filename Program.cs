﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        try{
                System.Diagnostics.Process batch = new System.Diagnostics.Process();
                batch.StartInfo.FileName = @"wwwroot\Run\disk.cmd";
                batch.Start();  
        }
        catch
        {;}        
        //https://stackoverflow.com/questions/4624113/start-a-net-process-as-a-different-user          



        return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(config)
            .UseStartup<Startup>();
        }

        public static IConfiguration AppConfig { get; set; }

        public static bool isPostgres {get;set;}
        public static bool FlagDadataUpdate = false;
    }
}
