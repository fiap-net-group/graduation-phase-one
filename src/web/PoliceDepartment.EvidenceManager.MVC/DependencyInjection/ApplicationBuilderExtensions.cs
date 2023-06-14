using PoliceDepartment.EvidenceManager.MVC.Authorization;

namespace PoliceDepartment.EvidenceManager.MVC.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseDependencyInjection(this WebApplication app)
        {
            app.UseMvcConfiguration();

            app.UseAuthorizationConfiguration();

            return app;
        }
    }
}
