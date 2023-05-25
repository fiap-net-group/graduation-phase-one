using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger
{
    [ExcludeFromCodeCoverage]
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();

                c.CustomSchemaIds(SchemaIdStrategy);

                c.IncludeCommentsToApiDocumentation();
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        private static void IncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            try
            {
                options.TryIncludeCommentsToApiDocumentation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TryIncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        }

        private static string SchemaIdStrategy(Type currentClass)
        {
            return currentClass.Name.Replace("ViewModel", string.Empty).Replace("Model", string.Empty);
        }

        internal static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
                {

                    foreach (var groupName in provider.ApiVersionDescriptions.Select(description => description.GroupName))
                        options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
                });

            return app;
        }
    }
}
