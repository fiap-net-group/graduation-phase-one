using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences
{
    public static class EvidencesExtensions
    {
        public static IServiceCollection AddEvidencesConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(ClientExtensions.EvidencesClientName, client =>
            {
                client.BaseAddress = new Uri(configuration["Api:Evidences:BaseAddress"]);
            });

            services.AddScoped<IEvidencesClient, EvidencesClient>();
            services.AddScoped<ICreateEvidence, EvidenceCreate>();

            return services;
        }
    }
}
