﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Dsana.Data;

namespace Dsana
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var dbContext = host.Services
                               .CreateScope().ServiceProvider
                                .GetRequiredService<ApplicationDbContext>();
            
            await dbContext.Database.MigrateAsync();
            await DataUtility.ManageDataAsync(host);
            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
