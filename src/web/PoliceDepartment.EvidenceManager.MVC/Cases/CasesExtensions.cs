using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.UseCases;

namespace PoliceDepartment.EvidenceManager.MVC.Cases
{
    public static class CasesExtensions
    {
        public static IServiceCollection AddCasesConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(ClientExtensions.CasesClientName, client =>
            {
                client.BaseAddress = new Uri(configuration["Api:Cases:BaseAddress"]);
            });

            services.AddScoped<ICasesClient, CasesClient>();
            services.AddScoped<IGetCasesByOfficerId, GetCasesByOfficerId>();
            services.AddScoped<IGetCaseDetails, GetCaseDetails>();
            services.AddScoped<ICreateCase, CreateCase>();

            return services;
        }
    }
}
