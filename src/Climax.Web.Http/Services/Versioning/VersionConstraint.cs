using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Http.Routing;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Services.Versioning
{
    public class VersionConstraint : IHttpRouteConstraint
    {
        private readonly int _version;

        public VersionConstraint(int version)
        {
            _version = version;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            var versionParser = request.GetConfiguration().Get<VersionParser>();
            if (versionParser == null)
            {
                throw new InvalidOperationException("You need to call ConfigureVersioning method against the HttpConfiguration first");
            }
            var version = versionParser.GetVersionFromRequest(request);
            return _version == version;
        }
    }
}