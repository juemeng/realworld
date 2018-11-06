using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moniter.Api.Hub;
using Moniter.Features.Deploy;
using Moniter.Features.Parser;
using Moniter.Features.PushNotification;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Security;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace Moniter.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var featureAssembly = Assembly.Load("Moniter.Features");
            services.AddMediatR(featureAssembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            services.AddDbContext<MoniterContext>(options =>
                options.UseMySql(_configuration.GetConnectionString(@"moniter")));
//            services.AddEntityFrameworkSqlite().AddDbContext<MoniterContext>();

            services.AddLocalization(x => x.ResourcesPath = "Resources");

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                x.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                });
                x.SwaggerDoc("v1", new Info {Title = "RealWorld API", Version = "v1"});
                x.CustomSchemaIds(y => y.FullName);
                x.DocInclusionPredicate((version, apiDescription) => true);
                x.TagActionsBy(y => y.GroupName);
            });

            services.AddCors();
            services.AddMvc(opt =>
                {
                    opt.Conventions.Add(new GroupByApiRootConvention());
                    opt.Filters.Add(typeof(ValidatorActionFilter));
                })
                .AddJsonOptions(opt => { opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssembly(featureAssembly); });

            services.AddAutoMapper(featureAssembly);

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<IAlertMessageParser, AlertMessageParse>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRemoteClient, RemoteClient>();
            services.Configure<PushSetting>(_configuration.GetSection("AliPush"));
            services.AddTransient<IAliyunPushClient, AliyunPushClient>();
            services.AddJwt();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilogLogging();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseSignalR(routes => { routes.MapHub<DeviceHub>("/deviceHub"); });

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets(HTML, JS, CSS etc.)
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "RealWorld API V1"); });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<MoniterContext>();
                context.Database.EnsureCreated();
                // Seed the database.
                SeedData.Initialize(serviceScope.ServiceProvider);
            }
        }
    }
}