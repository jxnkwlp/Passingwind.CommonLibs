using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class SwaggerEnumDescriptions : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (type.IsEnum)
            {
                var names = Enum.GetNames(type);

                //schema.Enum.Clear(); 
                //foreach (var item in names)
                //{
                //    schema.Enum.Add(new OpenApiInteger(Convert.ToInt32(Enum.Parse(type, item))));
                //}

                var enumKeyValue = new OpenApiArray();

                enumKeyValue.AddRange(names.Select(x => new OpenApiObject
                {
                    ["name"] = new OpenApiString(Convert.ToInt32(Enum.Parse(type, x)).ToString()),
                    ["value"] = new OpenApiString(x),
                }));

                var enumNames = new OpenApiArray();
                enumNames.AddRange(names.Select(x => new OpenApiString(x)));

                var enumValues = new OpenApiArray();
                enumValues.AddRange(names.Select(x => new OpenApiInteger(Convert.ToInt32(Enum.Parse(type, x)))));

                schema.Extensions.Add(
                    "x-enumNames",
                    enumNames
                );

                schema.Extensions.Add(
                    "x-enumValues",
                    enumValues
                );

                schema.Extensions.Add(
                    "x-ms-enum",
                    new OpenApiObject
                    {
                        ["name"] = new OpenApiString(type.Name),
                        ["modelAsString"] = new OpenApiBoolean(true),
                        ["values"] = enumKeyValue,
                    }
                );

                schema.Description = string.Join("<br/> ", names.Select(x => $"{Convert.ToInt32(Enum.Parse(type, x))}:{x}"));
            }
        }
    }
}
