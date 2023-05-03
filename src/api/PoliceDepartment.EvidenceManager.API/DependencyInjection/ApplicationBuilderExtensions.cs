using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseDependencyInjection(this WebApplication app)
        {
            app.UseApiConfiguration();

            app.UseSwaggerConfiguration();

            return app;
        }
    }
}
