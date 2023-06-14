using Microsoft.AspNetCore.Authentication.Cookies;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.MVC.Client;
using Polly;
using Polly.Extensions.Http;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization
{
    public static class AuthorizationExtensions
    {
        public const string AccessTokenClaimName = "AccessToken";

        public static void AddAuthorizationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/error/403";
                });

            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });


            services.AddHttpClient(ClientExtensions.AuthorizationClientName, client =>
            {
                client.BaseAddress = new Uri(configuration["Api:Authorization:BaseAddress"]);
            });

            services.AddSingleton(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Attempt: {retryCount}!");
                    Console.ForegroundColor = ConsoleColor.White;
                }));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationClient, AuthorizationClient>();
            services.AddScoped<IOfficerUser, OfficerUser>();
            services.AddScoped<ILogin, Login>();
            services.AddScoped<ILogout, Logout>();
        }

        public static void UseAuthorizationConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
