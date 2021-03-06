﻿namespace BuildBot
{
    using BuildBot.Eventing;
    using BuildBot.Projections;
    using BuildBot.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private const string CorsPolicy = "penfold";

        // This method gets called by the runtime. Use this method to add
        // services to the container. For more information on how to configure
        // your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IEventStore, ListStore>();
            services.AddSingleton<IComponentVersionProjection, ComponentVersionProjection>();
            services.AddSingleton<IComponentHistoryProjection, ComponentHistoryProjection>();
            services.AddAuthorization();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: CorsPolicy,
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:51836")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseGrpcWeb();
            app.UseCors(CorsPolicy);
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BuildService>();
                endpoints.MapGrpcService<ReleaseService>();
                endpoints.MapGrpcService<HistoryService>().EnableGrpcWeb();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}