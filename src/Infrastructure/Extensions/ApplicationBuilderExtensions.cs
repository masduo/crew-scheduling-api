using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CrewScheduling.Api.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary> Uses Swagger and SwaggerUI middlewares </summary>
        /// <param name="app"> The application builder that builds the request pipeline </param>
        /// <param name="versionDescriptionProvider"> The version description provider  </param>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider versionDescriptionProvider) =>
            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"Crew Scheduling API {description.GroupName}");
                    }
                });
    }
}
