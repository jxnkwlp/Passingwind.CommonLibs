using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void GenerateSchemaIdAndOperationId(this SwaggerGenOptions options)
        {
            //  
            options.SchemaFilter<SwaggerEnumDescriptions>();
            // 
            options.CustomSchemaIds(type =>
            {
                if (type.IsGenericType)
                {
                    string part1 = type.FullName.Substring(0, type.FullName.IndexOf("`")).RemovePostFix("Dto");
                    string part2 = string.Concat(type.GetGenericArguments().Select(x => x.Name.RemovePostFix("Dto")));

                    if (part1.EndsWith("ListResult") || part1.EndsWith("PagedResult"))
                    {
                        string temp1 = part1.Substring(0, part1.LastIndexOf("."));
                        string temp2 = part1.Substring(part1.LastIndexOf(".") + 1);
                        return $"{temp1}.{part2}{temp2}";
                    }

                    return $"{part1}.{part2}";
                }

                return type.FullName.RemovePostFix("Dto").Replace("Dto+", null);
            });
            //
            options.CustomOperationIds(e =>
            {
                string action = e.ActionDescriptor.RouteValues["action"];
                string controller = e.ActionDescriptor.RouteValues["controller"];
                string method = e.HttpMethod;

                if (action == "GetList")
                    return $"Get{controller}List";

                if (action == "GetAllList")
                    return $"GetAll{controller}List";

                if (action.StartsWith("GetAll"))
                    return $"GetAll{controller}{action.RemovePreFix("GetAll")}";

                if (action == "Get" || action == "Create" || action == "Update" || action == "Delete")
                    return action + controller;

                if (action.StartsWith("Get"))
                    return $"Get{controller}{action.RemovePreFix("Get")}";

                if (action.StartsWith("Create"))
                    return $"Create{controller}{action.RemovePreFix("Create")}";

                if (action.StartsWith("Update"))
                    return $"Update{controller}{action.RemovePreFix("Update")}";

                if (action.StartsWith("Delete"))
                    return $"Delete{controller}{action.RemovePreFix("Delete")}";

                if (action.StartsWith("BatchCreate"))
                    return $"BatchCreate{controller}{action.RemovePreFix("BatchCreate")}";

                if (action.StartsWith("BatchUpdate"))
                    return $"BatchUpdate{controller}{action.RemovePreFix("BatchUpdate")}";

                if (action.StartsWith("BatchDelete"))
                    return $"BatchDelete{controller}{action.RemovePreFix("BatchDelete")}";

                if (method == "HttpGet")
                    return action + controller;
                else
                    return controller + action;
            });
        }
    }
}