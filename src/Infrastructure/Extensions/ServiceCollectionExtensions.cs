using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace CrewScheduling.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> Adds API versioning service with default configurations </summary>
        /// <param name="services"> The collection of DI registered services </param>
        /// <param name="defaultApiVersion"> The default version </param>
        public static void AddApiVersioning(this IServiceCollection services, string defaultApiVersion)
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "VV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Parse(defaultApiVersion);
            });

            services.AddApiVersioning(options => options.ReportApiVersions = true);
        }

        /// <summary> Adds swagger generator service with xml documentation </summary>
        /// <param name="services"> The collection of DI registered services </param>
        /// <param name="defaultApiVersion"> The default version </param>
        public static void AddSwagger(this IServiceCollection services, string defaultApiVersion)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(defaultApiVersion, new OpenApiInfo
                {
                    Title = "Crew Scheduling API",
                    Version = defaultApiVersion,
                    Description = "Provides on-demand crew scheduling information to other services.",
                    Contact = new OpenApiContact { Url = new Uri("https://github.com/masduo/crew-scheduling-api") }
                });

                // camelCase querystring parameters
                options.DescribeAllParametersInCamelCase();

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CrewScheduling.Api.xml"));
            });
        }
    }
}
