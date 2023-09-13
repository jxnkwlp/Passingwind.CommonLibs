using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public static class SwaggerGenOptionsExtensions
    {
        private static readonly Dictionary<string, int> OperationIds = new Dictionary<string, int>();
        private static readonly Dictionary<ApiDescription, string> OperationIdCache = new Dictionary<ApiDescription, string>();

        public static void GenerateSchemaIdAndOperationId(this SwaggerGenOptions options, bool removeDtoFix = true, bool allowDuplicateOperationId = false)
        {
            //  
            options.SchemaFilter<SwaggerEnumDescriptions>();
            // 
            options.CustomSchemaIds(type =>
            {
                var typeString = type.ToSimpleTypeString(true);

                if (typeString.Contains("<"))
                {
                    var wrapType = typeString.Substring(0, typeString.IndexOf("<"));
                    var argType = typeString.Substring(typeString.IndexOf("<") + 1, typeString.Length - wrapType.Length - 2);

                    typeString = argType.Replace(", ", null) + wrapType;
                }

                if (removeDtoFix)
                    return typeString.Replace("Dto", null);

                return typeString;
            });
            //
            options.CustomOperationIds(e =>
            {
                string action = e.ActionDescriptor.RouteValues["action"];
                string controller = e.ActionDescriptor.RouteValues["controller"];
                string method = e.HttpMethod;

                if (OperationIdCache.ContainsKey(e))
                {
                    return OperationIdCache[e];
                }

                var operationId = GenerateApiOperationId(method, controller, action);

                if (!allowDuplicateOperationId)
                {
                    if (OperationIds.ContainsKey(operationId))
                    {
                        OperationIds[operationId]++;

                        operationId = operationId + "_" + OperationIds[operationId];
                    }
                    else
                    {
                        OperationIds[operationId] = 0;
                    }
                }

                OperationIdCache[e] = operationId;

                return operationId;
            });
        }

        private static string GenerateApiOperationId(string method, string controller, string action)
        {
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

            return controller + action;
        }
    }
}
