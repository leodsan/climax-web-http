using System;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace Climax.Web.Http.Services
{
    public class RouteDataValuesOnlyAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings,
            HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof (ValueProviderFactory), new RouteDataValueProviderFactory());
        }
    }
}