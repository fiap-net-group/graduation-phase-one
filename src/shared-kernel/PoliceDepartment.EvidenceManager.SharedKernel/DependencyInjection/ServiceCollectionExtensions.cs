using Microsoft.Extensions.DependencyInjection;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;

namespace PoliceDepartment.EvidenceManager.SharedKernel.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, ConsoleLogger>();

            return services;
        }
    }
}
