using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using PoliceDepartment.EvidenceManager.Infra.Identity;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using System.Text;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    public static class IdentityExtensions
    {
        internal static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IIdentityContext, IdentityContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));

            services.AddScoped<IIdentityContext, IdentityContext>();
            services.AddScoped<IIdentityManager, IdentityManager>();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {   
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }
            ).AddEntityFrameworkStores<IdentityContext>()
             .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicys();
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
                options.MapInboundClaims = false;
            });

            return services;
        }

        private static AuthorizationOptions AddPolicys(this AuthorizationOptions options)
        {
            options.AddPolicy(AuthorizationPolicies.IsAdmin, p =>
                        p.RequireAuthenticatedUser().RequireClaim("OfficerType", Enum.GetName(OfficerType.Administrator)));

            options.AddPolicy(AuthorizationPolicies.IsPoliceOfficer, p =>
                        p.RequireAuthenticatedUser().RequireClaim("OfficerType", Enum.GetName(OfficerType.Officer)));

            return options;
        }

        internal static WebApplication UseIdentityConfiguration(this WebApplication app)
        {
            app.UseAuthentication();

            app.UseAuthorization();

            return app;
        }
    }
}
