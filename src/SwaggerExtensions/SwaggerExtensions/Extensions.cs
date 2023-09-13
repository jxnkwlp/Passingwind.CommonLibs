using System;
using System.Collections.Generic;
using System.Linq;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    internal static class Extensions
    {
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

        public static string ToSimpleTypeString(this Type type, bool trimArgCount = true)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments().ToList();

                return HandleTypeArguments(type, trimArgCount, genericArgs);
            }

            return type.Name;
        }

        private static string HandleTypeArguments(Type t, bool trimArgCount, List<Type> availableArguments)
        {
            if (t.IsGenericType)
            {
                string value = t.Name;
                if (trimArgCount && value.IndexOf("`") > -1)
                {
                    value = value.Substring(0, value.IndexOf("`"));
                }

                if (t.DeclaringType != null)
                {
                    // This is a nested type, build the nesting type first
                    value = HandleTypeArguments(t.DeclaringType, trimArgCount, availableArguments) + "+" + value;
                }

                // Build the type arguments (if any)
                string argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                for (int i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ", ";

                    argString += ToSimpleTypeString(availableArguments[0], trimArgCount);
                    availableArguments.RemoveAt(0);
                }

                // If there are type arguments, add them with < >
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }

                return value;
            }

            return t.Name;
        }
    }
}
