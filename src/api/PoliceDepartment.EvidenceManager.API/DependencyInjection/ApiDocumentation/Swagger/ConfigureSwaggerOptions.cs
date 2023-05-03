using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger
{
    [ExcludeFromCodeCoverage]
    internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IConfiguration _configuration;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider,
                                       IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, _configuration));
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description,
                                                   IConfiguration configuration)
        {
            try
            {
                return TryCreateInfoForApiVersion(description, configuration);
            }
            catch (Exception)
            {
                return CreateInfoWithUndefinedInformations(description);
            }
        }

        private static OpenApiInfo TryCreateInfoForApiVersion(ApiVersionDescription description,
                                                             IConfiguration configuration)
        {
            if (!configuration.GetSection("Swagger").Exists())
            {
                return CreateInfoWithUndefinedInformations(description);
            }

            return CreateInfoWithDefinedInformations(description, configuration);
        }

        private static OpenApiInfo CreateInfoWithUndefinedInformations(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = Assembly.GetEntryAssembly().GetName().Name,
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
                info.Description += " This version is deprecated!";

            return info;
        }

        static OpenApiInfo CreateInfoWithDefinedInformations(ApiVersionDescription description,
                                                             IConfiguration configuration)
        {
            var info = new OpenApiInfo()
            {
                Title = configuration.GetValue<string>("Swagger:Title"),
                Version = description.ApiVersion.ToString(),
                Description = configuration.GetValue<string>("Swagger:Description"),
                Contact = new OpenApiContact()
                {
                    Name = configuration.GetValue<string>("Swagger:Contact:Name"),
                    Email = configuration.GetValue<string>("Swagger:Contact:Email")
                },
                License = new OpenApiLicense()
                {
                    Name = configuration.GetValue<string>("Swagger:License:Name"),
                    Url = new Uri(configuration.GetValue<string>("Swagger:License:Url"))
                }
            };

            if (description.IsDeprecated)
                info.Description += " This version is deprecated!";

            return info;
        }
    }
}