using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Passingwind.AspNetCore.Authentication.Saml2;

internal static class Extensions
{
    public static ITfoxtec.Identity.Saml2.Http.HttpRequest ToGenericHttpRequest(this HttpRequest request)
    {
        return new ITfoxtec.Identity.Saml2.Http.HttpRequest
        {
            Method = request.Method,
            QueryString = request.QueryString.Value,
            Query = ToNameValueCollection(request.Query),
            Form = "POST".Equals(request.Method, StringComparison.InvariantCultureIgnoreCase) ? ToNameValueCollection(request.Form) : null
        };
    }

    private static NameValueCollection ToNameValueCollection(IEnumerable<KeyValuePair<string, StringValues>> items)
    {
        NameValueCollection nv = new();
        foreach (KeyValuePair<string, StringValues> item in items)
        {
            nv.Add(item.Key, item.Value[0]);
        }
        return nv;
    }
}
