using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dsana.Models;
using Dsana.Services.Interfaces;
using Dsana.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Dsana.Services.Factories;
using Microsoft.AspNetCore.Http;

namespace Dsana
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    DataUtility.GetConnectionString(Configuration),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<DSUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<DSUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddScoped<IDSRolesService, DSRolesService>();
            services.AddScoped<IDSCompanyInfoService, DSCompanyInfoService>();
            services.AddScoped<IDSProjectService, DSProjectService>();
            services.AddScoped<IDSDTaskService, DSDTaskService>();
            services.AddScoped<IDSDTaskHistoryService, DSDTaskHistory>();
            services.AddScoped<IDSFileService, DSFileService>();
            services.AddScoped<IDSLookupService, DSLookupService>();

            services.AddScoped<IEmailSender, DSEmailService>();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
