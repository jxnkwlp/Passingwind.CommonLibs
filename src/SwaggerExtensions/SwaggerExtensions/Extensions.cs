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
    }
}