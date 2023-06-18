using PoliceDepartment.EvidenceManager.API.Middlewares;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    [ExcludeFromCodeCoverage]
    internal static class ApiExtensions
    {
        internal static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        internal static IApplicationBuilder UseApiConfiguration(this WebApplication app, bool isDevelopment)
        {
            app.UseHttpsRedirection();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();

            if(!isDevelopment)
            {
                app.UseMiddleware<ApiKeyMiddleware>();
            }

            return app;
        }
    }
}
