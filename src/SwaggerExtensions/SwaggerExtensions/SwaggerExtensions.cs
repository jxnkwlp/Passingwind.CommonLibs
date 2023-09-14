using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public static class SwaggerGenOptionsExtensions
    {
        private static readonly ConcurrentDictionary<string, int> OperationIdConflicts = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<ApiDescription, string> OperationIdCache = new ConcurrentDictionary<ApiDescription, string>();

        private static readonly ConcurrentDictionary<string, int> SchemaIdConflicts = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<Type, string> SchemaIdCache = new ConcurrentDictionary<Type, string>();

        [Obsolete("Use 'ApplyExtensions' ", error: true)]
        public static void GenerateSchemaIdAndOperationId(this SwaggerGenOptions options, bool removeDtoFix = true, bool allowDuplicateOperationId = false)
        {
            throw new NotImplementedException();
        }

        public static void ApplyExtensions(this SwaggerGenOptions swaggerGenOptions, SwaggerExtensionOptions options = null)
        {
            options = options ?? new SwaggerExtensionOptions();
            // 
            swaggerGenOptions.SchemaFilter<SwaggerEnumDescriptions>();
            // 
            swaggerGenOptions.CustomSchemaIds(type => GenerateSchemaId(type, options));
            //
            swaggerGenOptions.CustomOperationIds(e => GenerateOperationIds(e, options));
        }

        private static string GenerateSchemaId(Type type, SwaggerExtensionOptions options)
        {
            return SchemaIdCache.GetOrAdd(type, (typeItem) =>
            {
                var typeString = typeItem.ToSimpleTypeName();

                if (typeString.Contains("<"))
                {
                    var typeNames = typeString.Split('<', '>').ToList();
                    typeNames.Reverse();

                    typeString = string.Concat(typeNames);
                }

                if (options.RemoveDtoFix)
                    typeString = typeString.Replace("Dto", null);

                if (options.ConflictingSchemaIdResolver == null)
                {
                    options.ConflictingSchemaIdResolver = (_, s) => HandleConflictingTypeSchemaId(s);
                }

                return options.ConflictingSchemaIdResolver(typeItem, typeString);
            });
        }

        private static string HandleConflictingTypeSchemaId(string schemaId)
        {
            if (SchemaIdConflicts.ContainsKey(schemaId))
            {
                SchemaIdConflicts[schemaId]++;
                return schemaId + SchemaIdConflicts[schemaId];
            }

            SchemaIdConflicts[schemaId] = 0;
            return schemaId;
        }

        private static string GenerateOperationIds(ApiDescription apiDescription, SwaggerExtensionOptions options)
        {
            return OperationIdCache.GetOrAdd(apiDescription, (apiItem) =>
            {
                string action = apiItem.ActionDescriptor.RouteValues["action"];
                string controller = apiItem.ActionDescriptor.RouteValues["controller"];
                string method = apiItem.HttpMethod;

                var operationId = GenerateApiOperationId(method, controller, action);

                if (!options.AllowDuplicateOperationId)
                {
                    if (OperationIdConflicts.ContainsKey(operationId))
                    {
                        OperationIdConflicts[operationId]++;

                        operationId += OperationIdConflicts[operationId];
                    }
                    else
                    {
                        OperationIdConflicts[operationId] = 0;
                    }
                }

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
