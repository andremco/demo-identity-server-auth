﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using IdentityServer.Middlewares;
using IdentityServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

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

            if (Environment.IsDevelopment())
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                LoadCertificate(builder);
            }

            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();

            RegisterServices(services);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));
            services.Configure<AzureKeyVaultCert>(Configuration.GetSection("AzureKeyVault"));
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

        public async void LoadCertificate(IIdentityServerBuilder builder)
        {
            /*
                Article - Azure Key Vault Certificate client library for .NET
                https://docs.microsoft.com/en-us/dotnet/api/overview/azure/security.keyvault.certificates-readme-pre
                Github - Setting, getting, updating, and deleting certificates
                https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/keyvault/Azure.Security.KeyVault.Certificates/samples/Sample1_HelloWorld.md
                Github - Download a certificate containing the private key
                https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/keyvault/Azure.Security.KeyVault.Certificates/samples/Sample4_DownloadCertificate.md        
                Article - CREATE CERTIFICATES FOR IDENTITYSERVER4 SIGNING USING .NET CORE
                https://damienbod.com/2020/02/10/create-certificates-for-identityserver4-signing-using-net-core/
             */

            var keyVaultName = Configuration.GetSection("AzureKeyVault:KeyVaultUri").Value;
            var certificateName = Configuration.GetSection("AzureKeyVault:CertName").Value;
            CertificateClient client = new CertificateClient(new Uri(keyVaultName), new DefaultAzureCredential());
            using X509Certificate2 certificate = await client.DownloadCertificateAsync(certificateName);

            ECDsa key = certificate.GetECDsaPrivateKey();

            ECDsaSecurityKey ecdsaCertificatePublicKey = new ECDsaSecurityKey(key);

            builder.AddSigningCredential(ecdsaCertificatePublicKey, IdentityServer4.IdentityServerConstants.ECDsaSigningAlgorithm.ES256);
        }
    }
}
