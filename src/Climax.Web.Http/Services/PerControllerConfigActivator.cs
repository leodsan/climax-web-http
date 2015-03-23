using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Services
{
    public class PerControllerConfigActivator : IHttpControllerActivator
    {
        private static readonly DefaultHttpControllerActivator Default = new DefaultHttpControllerActivator();

        private readonly ConcurrentDictionary<Type, HttpConfiguration> _cache =
            new ConcurrentDictionary<Type, HttpConfiguration>();

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            HttpConfiguration controllerConfig;
            if (_cache.TryGetValue(controllerType, out controllerConfig))
            {
                controllerDescriptor.Configuration = controllerConfig;
            }
            else
            {
                var configMap = request.GetConfiguration().GetControllerConfigurationMap();
                if (configMap != null && configMap.ContainsKey(controllerType))
                {
                    controllerDescriptor.Configuration =
                        controllerDescriptor.Configuration.Copy(configMap[controllerType]);
                    _cache.TryAdd(controllerType, controllerDescriptor.Configuration);
                }
            }

            var result = Default.Create(request, controllerDescriptor, controllerType);
            return result;
        }
    }
}