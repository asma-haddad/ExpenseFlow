using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExpenseFlow.Domain.Shared.Attribute
{
    public class AcceptLanguageHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var SupportLanguage = new List<string>() { "ar", "en" };

            if (operation.Parameters == null)
            {
                // operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.String,
                    Example = "en",

                    // Cast each OpenApiString to IOpenApiAny so the Enum property accepts the collection
                    // Enum = SupportLanguage.Select(x => (IOpenApiAny)new OpenApiString(x)).ToList()
                },
                Required = false,
            });
        }
    }
}
