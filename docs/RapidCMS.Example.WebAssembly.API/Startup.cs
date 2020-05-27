﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.WebAssembly.API
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
            services.AddScoped<JsonRepository<Person>>();
            services.AddScoped<JsonRepository<ConventionalPerson>>();
            services.AddScoped<JsonRepository<Country>>();
            services.AddScoped<JsonRepository<User>>();
            services.AddScoped<JsonRepository<TagGroup>>();
            services.AddScoped<JsonRepository<Tag>>();

            services.AddRapidCMSApi(config =>
            {
                // TODO: missing configurations:
                // - update name to AddCollection to be inline with the rest
                // - DataView / DataViewBuilder
                // - 
                config.RegisterRepository<Person, JsonRepository<Person>>("person");
                config.RegisterRepository<ConventionalPerson, JsonRepository<ConventionalPerson>>("person-convention");
                config.RegisterRepository<Country, JsonRepository<Country>>("country");
                config.RegisterRepository<User, JsonRepository<User>>("user");
                config.RegisterRepository<TagGroup, JsonRepository<TagGroup>>("taggroup");
                config.RegisterRepository<Tag, JsonRepository<Tag>>("tag");

                config.AllowAnonymousUser();
            });

            services.AddCors();
            services
                .AddControllers(config =>
                {
                    config.Conventions.AddRapidCMSRouteConvention();
                })
                .AddNewtonsoftJson()
                .ConfigureApplicationPartManager(configure =>
                {
                    configure.FeatureProviders.AddRapidCMSControllerFeatureProvider();
                });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                .WithOrigins("https://localhost:5001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}