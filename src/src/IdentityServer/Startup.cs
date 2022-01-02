// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Middlewares;
using IdentityServer.Settings;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            //To get value of secret to set in appsettings file
            //var secret = new Secret("b8b66f0a8781609214dc1fac0c43c5284491cbc00dfcc968ebbe834df3022894".Sha256());

            var builder = services
                .AddIdentityServer(options =>
                {
                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Configuration.GetSection("IdentityServer:IdentityResources"))
                .AddInMemoryApiScopes(Configuration.GetSection("IdentityServer:ApiScopes"))
                .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
                .AddTestUsers(TestUsers.Users);
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();

            RegisterServices(services);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApplicationInsightsSettings> options)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseMiddleware<LogMiddleware>();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ErrorHandlerMiddleware(options, env).Invoke
            });
        }
    }
}
