using Microsoft.Extensions.DependencyInjection;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;

namespace PoliceDepartment.EvidenceManager.SharedKernel.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, ConsoleLogger>();

            return services;
        }
    }
}
