using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.DependencyInjection;

namespace PoliceDepartment.EvidenceManager.MVC.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvcConfiguration();
            
            services.AddAuthorizationConfiguration(configuration);

            services.AddCustomLogging();

            return services;
        }
    }
}
