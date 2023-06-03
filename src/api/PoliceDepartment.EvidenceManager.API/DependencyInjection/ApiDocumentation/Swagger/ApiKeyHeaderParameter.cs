using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.ApiDocumentation.Swagger
{
    public class ApiKeyHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-API-KEY",
                In = ParameterLocation.Header,
                Description = "Key to access the API",
                Required = false,
                Schema = default
            });
        }
    }
}
