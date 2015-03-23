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

        public static void ConfigureDefaultVersioning(this HttpConfiguration configuration)
        {
            var parser = new VersionParser();
            configuration.Properties[typeof(IVersionParser)] = parser;
        }

        /// <summary>
        /// Configure versioning - header and media types. Pass null to ignore a given versioning mechanism.
        /// </summary>
        /// <param name="configuration"> HTTP configuration</param>
        /// <param name="versioningHeaderName"> Name of header used for versioning</param>
        /// <param name="vesioningMediaTypes"> Collection of media types subject to versioning. You just register the base here i.e. "application/vnd.climax". Then in the HTTP requests, the required Accept header format is to delimit version number with "-v" and then "+", i.e. application/vnd.climax-v2+json or application/vnd.climax-v3+xml.</param>
        public static void ConfigureVersioning(this HttpConfiguration configuration, string versioningHeaderName, string[] vesioningMediaTypes)
        {
            var parser = new VersionParser(versioningHeaderName, vesioningMediaTypes);
            configuration.Properties[typeof(IVersionParser)] = parser;
        }

        public static void ConfigureVersioning(this HttpConfiguration configuration, IVersionParser parser)
        {
            configuration.Properties[typeof(IVersionParser)] = parser;
        }
    }
}