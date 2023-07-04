namespace PoliceDepartment.EvidenceManager.MVC.DependencyInjection
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
        {
            services.AddControllersWithViews();

            return services;
        }

        internal static IApplicationBuilder UseMvcConfiguration(this WebApplication app)
        {
            app.UseExceptionHandler("/error/500");
            app.UseStatusCodePagesWithRedirects("/error/{0}");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/");

            return app;
        }
    }
}
