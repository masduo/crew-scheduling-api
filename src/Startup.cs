using CrewScheduling.Api.Handlers.QueryHandlers;
using CrewScheduling.Api.Infrastructure.Extensions;
using CrewScheduling.Api.Stores;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrewScheduling.Api
{
    public class Startup
    {
        public const string DefaultApiVersion = "1.0";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(DefaultApiVersion);

            services.AddSwagger(DefaultApiVersion);

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddHealthChecks();

            services
                .AddTransient<IPilotReader, PilotReader>()
                .AddTransient<IScheduleReader, ScheduleReader>()
                .AddTransient<IScheduleWriter, ScheduleWriter>();

            services.AddMediatR(typeof(AvailabilityQueryHandler));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider vdp)
        {
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });

            app.UseSwagger(vdp);
        }
    }
}
