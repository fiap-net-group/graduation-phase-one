using PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger;
using PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Infra.Database;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using PoliceDepartment.EvidenceManager.Infra.Identity;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseDependencyInjection(this WebApplication app, bool isDevelopment)
        {
            app.UseApiConfiguration(isDevelopment);

            app.UseIdentityConfiguration();

            app.UseSwaggerConfiguration();

            app.ApplyMigrations();

            return app;
        }

        internal static IApplicationBuilder ApplyMigrations(this WebApplication app)
        {
            using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

            using var databaseContext = serviceScope.ServiceProvider.GetRequiredService<SqlServerContext>();
            using var identityContext = serviceScope.ServiceProvider.GetRequiredService<IdentityContext>();

            if (databaseContext.AnyPendingMigrationsAsync().Result)
                databaseContext.MigrateAsync().Wait();

            if (identityContext.AnyPendingMigrationsAsync().Result)
                identityContext.MigrateAsync().Wait();

            return app;
        }
    }
}
