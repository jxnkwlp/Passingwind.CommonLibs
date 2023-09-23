using System;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class SwaggerExtensionOptions
    {
        public bool RemoveDtoFix { get; set; } = true;
        public bool AllowDuplicateOperationId { get; set; }
        public Func<Type, string, string> ConflictingSchemaIdResolver { get; set; }
    }
}
