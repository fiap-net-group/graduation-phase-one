using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using PoliceDepartment.EvidenceManager.Infra.Database.Repositories;
using PoliceDepartment.EvidenceManager.Infra.FileManager;
using PoliceDepartment.EvidenceManager.Infra.Identity;
using PoliceDepartment.EvidenceManager.Infra.Logger;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    [ExcludeFromCodeCoverage]
    internal static class InfraExtensions
    {
        internal static IServiceCollection AddInfraConfiguration(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddScoped<IAppDatabaseContext, SqlServerContext>();
            services.AddScoped<IIdentityContext, IdentityContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEvidenceRepository, EvidenceRepository>();
            services.AddScoped<ICaseRepository, CaseRepository>();
            services.AddScoped<IOfficerRepository, OfficerRepository>();

            services.AddTransient(o => new SqlConnection(configuration.GetConnectionString("SqlServerConnection")));
            services.AddDbContext<SqlServerContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));

            if (isDevelopment)
            {
                services.AddScoped<IEvidenceFileServer, EvidenceLocalServer>();
                services.AddSingleton<ILoggerManager, ConsoleLogger>();
            }
            else
            {
                //ADD PROD DI
            }


            return services;
        }

        internal static WebApplication UseInfrastructureConfiguration(this WebApplication app)
        {
            using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

            using var databaseContext = serviceScope.ServiceProvider.GetRequiredService<IAppDatabaseContext>();
            using var identityContext = serviceScope.ServiceProvider.GetRequiredService<IIdentityContext>();

            if (databaseContext.AnyPendingMigrationsAsync().Result)
                databaseContext.MigrateAsync();

            if (identityContext.AnyPendingMigrationsAsync().Result)
                identityContext.MigrateAsync();

            return app;
        }
    }
}
