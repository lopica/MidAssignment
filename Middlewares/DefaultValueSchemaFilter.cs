using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;

namespace MidAssignment.Middlewares
{
    public class DefaultValueSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var property in context.Type.GetProperties())
            {
                var defaultValueAttr = property.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                                               .Cast<DefaultValueAttribute>()
                                               .FirstOrDefault();

                if (defaultValueAttr != null && schema.Properties.ContainsKey(property.Name))
                {
                    var defaultValue = defaultValueAttr.Value;

                    if (defaultValue is bool boolVal)
                    {
                        schema.Properties[property.Name].Default = new OpenApiBoolean(boolVal);
                    }
                    else if (defaultValue is int intVal)
                    {
                        schema.Properties[property.Name].Default = new OpenApiInteger(intVal);
                    }
                    else if (defaultValue is string strVal)
                    {
                        schema.Properties[property.Name].Default = new OpenApiString(strVal);
                    }
                    else if (defaultValue is double doubleVal)
                    {
                        schema.Properties[property.Name].Default = new OpenApiDouble(doubleVal);
                    }
                    // Add other types as needed
                }
            }
        }
    }
}
