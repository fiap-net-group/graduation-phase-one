using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddApiConfiguration();

            services.AddInfraConfiguration(configuration, isDevelopment);

            services.AddApiVersioningConfiguration();

            services.AddSwaggerConfiguration();

            return services;
        }
    }
}
