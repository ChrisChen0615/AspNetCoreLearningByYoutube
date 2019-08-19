﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLearningByYoutube.Models;
using CoreLearningByYoutube.Models.DISample;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLearningByYoutube
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // 想像這是DI容器，內容就是inject 服務
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940        
        public void ConfigureServices(IServiceCollection services)
        {
            //加入資料庫            
            services.AddDbContextPool<AppDbContext>(
                //建立provider
                options => options.UseSqlServer(_config.GetConnectionString("DefaultDBConnection"))
                .EnableSensitiveDataLogging());

            //建立indentity dbcontext
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                //options.Password.RequiredUniqueChars = 1;
            }).AddEntityFrameworkStores<AppDbContext>();

            ////建立密碼複雜度(可建立在addidentity內)
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 5;
            //    //options.Password.RequiredUniqueChars = 1;
            //});

            //產出格式為xml
            //services.AddMvc().AddXmlSerializerFormatters();

            //加入mvc服務
            services.AddMvc(options =>
            {
                //全域授權要求
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
            //直接替換repository
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            //services.AddScoped<IEmployeeRepository, MockEmployeeRepository>();
            //services.AddTransient<IEmployeeRepository, MockEmployeeRepository>();

            /*
             * 以下為dependcy injection三種方法範例
             * Transient:every instance is new
             * Scoped:use the same instance within the same http request
             * Singleton: use the same instance within all http requests
             */
            services.AddTransient<ISampleTransient, Sample>();
            services.AddScoped<ISampleScoped, Sample>();
            services.AddSingleton<ISampleSingleton, Sample>();
            services.AddTransient<SampleService, SampleService>();
        }

        // middleware，設定request pipeline
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                //處理error方式
                //1.
                //app.UseStatusCodePages();
                //2.
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                //3.
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();

            //identity
            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}