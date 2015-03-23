using System.Linq;
using System.Net.Http;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Services.Versioning
{
    public class VersionFinder
    {
        private static readonly string ApiVersion = "ApiVersion";

        private static bool NeedsAcceptVersioning(HttpRequestMessage request, out string version)
        {
            if (request.Headers.Accept.Any())
            {
                var acceptHeaderVersion =
                    request.Headers.Accept.FirstOrDefault(x => x.MediaType.ToLowerInvariant().Contains(MediaTypes.Climax));

                if (acceptHeaderVersion != null && acceptHeaderVersion.MediaType.Contains("-v") &&
                    acceptHeaderVersion.MediaType.Contains("+"))
                {
                    version = acceptHeaderVersion.MediaType.Between("-v", "+");
                    return true;
                }
            }

            version = null;
            return false;
        }

        private static bool NeedsHeaderVersioning(HttpRequestMessage request, out string version)
        {
            if (request.Headers.Contains(ApiVersion))
            {
                version = request.Headers.GetValues(ApiVersion).FirstOrDefault();
                if (version != null)
                {
                    return true;
                }
            }

            version = null;
            return false;
        }

        private static int VersionToInt(string versionString)
        {
            int version;
            if (string.IsNullOrEmpty(versionString) || !int.TryParse(versionString, out version))
                return 0;

            return version;
        }

        public int GetVersionFromRequest(HttpRequestMessage request)
        {
            string version;
            if (NeedsAcceptVersioning(request, out version))
            {
                return VersionToInt(version);
            }

            if (NeedsHeaderVersioning(request, out version))
            {
                return VersionToInt(version);
            }

            return 0;
        }
    }
}