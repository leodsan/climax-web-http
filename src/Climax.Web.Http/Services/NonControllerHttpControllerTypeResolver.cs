using System;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Climax.Web.Http.Services
{
    public class NonControllerHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        public NonControllerHttpControllerTypeResolver()
            : base(IsHttpEndpoint)
        {
            var suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
            if (suffix != null) suffix.SetValue(null, string.Empty);
        }

        internal static bool IsHttpEndpoint(Type t)
        {
            if (t == null) throw new ArgumentNullException("t");

            return t.IsClass && t.IsVisible && !t.IsAbstract && typeof(IHttpController).IsAssignableFrom(t);
        }
    }
}