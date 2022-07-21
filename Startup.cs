using E2ECHATAPI.Services.MessageServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "E2ECHATAPI", Version = "v1" });
                c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Description = "Api key needed to access the endpoints. (eg.) ApiKey: 12345",
                    In = ParameterLocation.Header,
                    Name = "ApiKey",
                    Type = SecuritySchemeType.ApiKey
                });
            });
            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("cors", builder => builder.WithOrigins("*", "http://localhost", "http://localhost:3000")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true));
            });

            

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ChatHubAuthPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes = new List<string> { "ChatHubAuthScheme" };
                });
            });


            services.AddAuthentication()
            .AddScheme<ChatHubAuthSchemeOptions, ChatHubAuthHandler>("ChatHubAuthScheme", options => { });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.StreamBufferCapacity = 30;
                hubOptions.MaximumReceiveMessageSize = int.MaxValue;
                hubOptions.MaximumParallelInvocationsPerClient = 100;
            }).AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseCors("cors");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E2ECHATAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/e2echat", (e) =>
                {
                    e.Transports =
                    HttpTransportType.WebSockets |
                    HttpTransportType.LongPolling |
                    HttpTransportType.ServerSentEvents;
                    e.TransportMaxBufferSize = int.MaxValue;
                    e.ApplicationMaxBufferSize = int.MaxValue;
                });
            });
        }
    }
}
