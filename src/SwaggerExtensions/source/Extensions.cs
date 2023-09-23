using System;
using System.Collections.Concurrent;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    internal static class Extensions
    {
        private static readonly ConcurrentDictionary<Type, string> TypeSimpleNameCache = new ConcurrentDictionary<Type, string>();

        public static string RemovePostFix(this string value, string postfix)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.EndsWith(postfix))
                return value.Substring(0, value.Length - postfix.Length);

            return value;
        }

        public static string RemovePreFix(this string value, string prefix)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.StartsWith(prefix))
                return value.Substring(prefix.Length);

            return value;
        }

        public static string ToSimpleTypeName(this Type type)
        {
            return TypeSimpleNameCache.GetOrAdd(type, (t) => BuildSimpleTypeName(t));
        }

        private static string BuildSimpleTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();

                string name = type.Name;
                if (name.IndexOf("`") > -1)
                {
                    name = name.Substring(0, name.IndexOf("`"));
                }

                sb.Append(name);

                var argumenTypes = type.GetGenericArguments();
                if (argumenTypes.Length > 0)
                {
                    sb.Append("<");
                    foreach (var item in argumenTypes)
                    {
                        sb.Append(BuildSimpleTypeName(item));
                    }
                    sb.Append(">");
                }

                return sb.ToString();
            }

            return type.Name;
        }
    }
}
