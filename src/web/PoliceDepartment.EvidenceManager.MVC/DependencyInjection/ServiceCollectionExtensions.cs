using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.MVC.Cases;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.SharedKernel.DependencyInjection;

namespace PoliceDepartment.EvidenceManager.MVC.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvcConfiguration()
                    .AddClientConfiguration()
                    .AddAuthorizationConfiguration(configuration)
                    .AddCasesConfiguration(configuration)
                    .AddCustomLogging();

            return services;
        }
    }
}
