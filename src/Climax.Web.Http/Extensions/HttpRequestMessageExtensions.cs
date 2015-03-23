using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Climax.Web.Http.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        public static bool IsLocal(this HttpRequestMessage request)
        {
            var localFlag = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return localFlag != null && localFlag.Value;
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            //Web-hosting
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            //Self-hosting
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            //Owin-hosting
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic ctx = request.Properties[OwinContext];
                if (ctx != null)
                {
                    return ctx.Request.RemoteIpAddress;
                }
            }
            return null;
        }

        public static T Get<T>(this HttpRequestMessage request, string key)
        {
            object value;
            request.Properties.TryGetValue(key, out value);

            return value != null ? (T)value : default(T);
        }

        public static string GetHeaderValue(this HttpRequestMessage requestMessage, string key)
        {
            IEnumerable<string> values;
            if (requestMessage.Headers.TryGetValues(key, out values) && values != null)
            {
                return values.FirstOrDefault();
            }

            return null;
        }

        public static T GetPropertyValue<T>(this HttpRequestMessage requestMessage, string key)
        {
            if (requestMessage.Properties.ContainsKey(key))
            {
                var obj = requestMessage.Properties[key];
                if (obj != null)
                {
                    try
                    {
                        return (T)obj;
                    }
                    catch
                    {
                    }
                }
            }

            return default(T);
        }
    }
}
