using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules;
using PoliceDepartment.EvidenceManager.SharedKernel.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddApiConfiguration();

            services.AddApplicationConfiguration();

            services.AddInfraConfiguration(configuration, isDevelopment);

            services.AddIdentityConfiguration(configuration);

            services.AddCustomLogging();

            services.AddApiVersioningConfiguration();

            services.AddSwaggerConfiguration();

            return services;
        }
    }
}
