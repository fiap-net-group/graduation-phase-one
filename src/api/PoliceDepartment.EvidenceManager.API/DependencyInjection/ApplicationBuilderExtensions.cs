using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseDependencyInjection(this WebApplication app, bool isDevelopment)
        {
            app.UseInfrastructureConfiguration();

            app.UseIdentityConfiguration();

            app.UseApiConfiguration(isDevelopment);

            app.UseSwaggerConfiguration();

            return app;
        }
    }
}
