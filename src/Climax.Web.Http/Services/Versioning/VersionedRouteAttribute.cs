using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Climax.Web.Http.Services.Versioning
{
    public class VersionedRouteAttribute : RouteFactoryAttribute
    {
        public VersionedRouteAttribute(string template)
            : base(template)
        {
            Order = -1;
        }

        public int Version { get; set; }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                return new HttpRouteValueDictionary
                {
                    {string.Empty, new VersionConstraint(Version)}
                };
            }
        }
    }
}
