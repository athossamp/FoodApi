using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace log_food_api
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpenApiRequestBodyType : Attribute
    {
        public Type BodyType { get; }
        public string[] ContentTypes { get; }
        public OpenApiRequestBodyType(Type type, string[] contentTypes = null)
        {
            BodyType = type;
            ContentTypes = contentTypes;
        }
    }
    public class SwaggerBodyTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var bodyTypeAttribute = context.ApiDescription.CustomAttributes().OfType<OpenApiRequestBodyType>().FirstOrDefault();

            if (bodyTypeAttribute != null)
            {
                var schema = context.SchemaGenerator.GenerateSchema(bodyTypeAttribute.BodyType, context.SchemaRepository);

                operation.RequestBody = new OpenApiRequestBody();

                string[] contentTypes;
                if (bodyTypeAttribute.ContentTypes != null)
                    contentTypes = bodyTypeAttribute.ContentTypes;
                else
                    contentTypes = operation.Responses.Where(x => x.Key == "200").SelectMany(x => x.Value.Content).Select(x => x.Key).ToArray();

                foreach (var contentType in contentTypes)
                {
                    operation.RequestBody.Content.Add(KeyValuePair.Create(contentType, new OpenApiMediaType { Schema = schema }));
                }
            }
        }
    }
}
