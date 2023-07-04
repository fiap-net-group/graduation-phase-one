using Microsoft.AspNetCore.Authentication.Cookies;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.MVC.Client;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization
{
    public static class AuthorizationExtensions
    {
        public const string AccessTokenClaimName = "AccessToken";

        public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/error/403";
                });

            services.AddHttpClient(ClientExtensions.AuthorizationClientName, client =>
            {
                client.BaseAddress = new Uri(configuration["Api:Authorization:BaseAddress"]);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationClient, AuthorizationClient>();
            services.AddScoped<IOfficerUser, OfficerUser>();
            services.AddScoped<ILogin, Login>();
            services.AddScoped<ILogout, Logout>();

            return services;
        }

        public static IApplicationBuilder UseAuthorizationConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
