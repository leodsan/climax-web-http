using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using Climax.Web.Http.Services;
using Climax.Web.Http.Services.Versioning;

namespace Climax.Web.Http.Extensions
{
    public static class HttpConfigurationExtensions
    {
        public static Dictionary<Type, Action<HttpControllerSettings>> Map = new Dictionary<Type, Action<HttpControllerSettings>>();

        public static T Get<T>(this HttpConfiguration configuration) where T : class
        {
            object value;
            configuration.Properties.TryGetValue(typeof(T), out value);

            return value as T;
        }

        public static T Get<T>(this HttpConfiguration configuration, object key) where T : class
        {
            object value;
            configuration.Properties.TryGetValue(key, out value);

            return value as T;
        }

        public static void InjectInterfacesIntoActions(this HttpConfiguration config)
        {
            config.ParameterBindingRules.Insert(0, param =>
            {
                if (param.ParameterType.IsInterface)
                {
                    return new InjectParameterBinding(param);
                }

                return null;
            });
        }

        public static HttpConfiguration Copy(this HttpConfiguration configuration, Action<HttpControllerSettings> settings)
        {
            var controllerSettings = new HttpControllerSettings(configuration);
            settings(controllerSettings);

            var constructor = typeof(HttpConfiguration).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(HttpConfiguration), typeof(HttpControllerSettings) }, null);
            var instance = (HttpConfiguration)constructor.Invoke(new object[] { configuration, controllerSettings });

            return instance;
        }

        public static void AddControllerConfigurationMap(this HttpConfiguration configuration,
            Dictionary<Type, Action<HttpControllerSettings>> controllerFongiurationMap)
        {
            configuration.Properties["controllerConfigMap"] = controllerFongiurationMap;
        }

        public static Dictionary<Type, Action<HttpControllerSettings>> GetControllerConfigurationMap(
            this HttpConfiguration configuration)
        {
            var map = configuration.Properties["controllerConfigMap"];
            return map as Dictionary<Type, Action<HttpControllerSettings>>;
        }

        public static void ConfigureVersioning(string versioningHeaderName, string[] vesioningMediaTypes)
        {
            VersionFinder.ApiVersion = versioningHeaderName;
            VersionFinder.AcceptMediaTypes = vesioningMediaTypes;
        }
    }
}