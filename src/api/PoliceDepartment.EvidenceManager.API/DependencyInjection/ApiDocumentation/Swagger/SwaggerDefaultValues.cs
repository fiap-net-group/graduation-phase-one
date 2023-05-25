using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger
{
    [ExcludeFromCodeCoverage]
    internal sealed class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated |= apiDescription.IsDeprecated();

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
                var response = operation.Responses[responseKey];

                foreach (var contentType in from contentType in response.Content.Keys
                                            where responseType.ApiResponseFormats.All(x => x.MediaType != contentType)
                                            select contentType)
                {
                    response.Content.Remove(contentType);
                }
            }

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                parameter.Description ??= description.ModelMetadata.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}