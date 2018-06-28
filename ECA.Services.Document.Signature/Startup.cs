using System.Net.Http;
using ECA.Services.Document.Signature.Config;
using ECA.Services.Document.Signature.DocuSign;
using ECA.Services.Document.Signature.DocuSign.Builders;
using ECA.Services.Document.Signature.Models;
using ECA.Services.Document.Signature.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ECA.Services.Document.Signature.DAL;
using Microsoft.EntityFrameworkCore;
using ECA.Services.Document.Signature.Security;
using NLog.Web;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace ECA.Services.Document.Signature
{
    public class Startup
    {
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration configuration)       // ctor
        {
            var builder = new ConfigurationBuilder()        // change suggested with impl of NLog
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();                // 
            //Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("Get", "Post", "Put")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc().AddJsonOptions( options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // task manager  (for background processing)
            services.AddSingleton<IHostedService, TaskManager>();  

            // Swagger - autodocument setup
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Signature Service",
                    Version = "v1",
                    Description = "API methods provided by Signature Service",
                    TermsOfService = "(C) 2018 Epiq  All Rights Reserved."
                });
            });

            // DI Dependency injection - built into ASPNETCore 
            services.AddTransient<IResponse, Response>();
            services.AddTransient<HttpClient>();
            services.AddTransient<IJsonConfiguration, JsonConfiguration>();
            services.AddTransient<IRestApiWrapper, RestApiWrapper>();
            services.AddTransient<ISignatureBuilder, SignatureBuilder>();
            services.AddTransient<ITemplateFieldBuilder, TemplateFieldBuilder>();
            services.AddTransient<IStatusBuilder, StatusBuilder>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IDocuSignStatusUpdater, DocuSignStatusUpdater>();

            // database context setup
            IJsonConfiguration config = new JsonConfiguration();
            string connection = config.ConnectionString;
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger- autodocument
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Signature Service");
            });

            // JWT/OAuth pipeline
            app.UseMiddleware<AuthenticationMiddleware>(new JsonConfiguration());


            app.UseCors("CorsPolicy");

            app.UseMvc();

            
            // Nlog database log file

            var defaultConnection = Configuration.GetConnectionString("NLogDb");
            NLog.GlobalDiagnosticsContext.Set("defaultConnection", defaultConnection );

            env.ConfigureNLog("nlog.config");
            loggerFactory.AddNLog();

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Info("Signature service started.");


        }
    }
}
