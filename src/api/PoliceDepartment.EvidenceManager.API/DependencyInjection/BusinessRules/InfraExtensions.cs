using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using PoliceDepartment.EvidenceManager.Infra.Database.Repositories;
using PoliceDepartment.EvidenceManager.Infra.FileManager;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    [ExcludeFromCodeCoverage]
    internal static class InfraExtensions
    {
        internal static IServiceCollection AddInfraConfiguration(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddScoped<IAppDatabaseContext, SqlServerContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEvidenceRepository, EvidenceRepository>();
            services.AddScoped<ICaseRepository, CaseRepository>();
            services.AddScoped<IOfficerRepository, OfficerRepository>();

            services.AddTransient(o => new SqlConnection(configuration.GetConnectionString("SqlServerConnection")));
            services.AddDbContext<SqlServerContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));
            services.AddLogging();

            services.AddScoped<IEvidenceFileServer>(_ => new EvidenceFileServer(new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"))));


            return services;
        }
    }
}
