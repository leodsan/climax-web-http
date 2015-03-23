using System.Linq;
using System.Net.Http;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Services.Versioning
{
    public class VersionParser : IVersionParser
    {
        private readonly string _apiVersionHeader;
        private readonly string[] _mediaTypes;

        public VersionParser() : this("ApiVersion",  new [] {MediaTypes.Climax})
        {
        }

        public VersionParser(string apiVersionHeader, string[] mediaTypes)
        {
            _apiVersionHeader = apiVersionHeader;
            _mediaTypes = mediaTypes;
        }

        protected virtual bool NeedsAcceptVersioning(HttpRequestMessage request, out string version)
        {
            if (_mediaTypes != null && _mediaTypes.Any() && request.Headers.Accept.Any())
            {
                var acceptHeaderVersion =
                    request.Headers.Accept.FirstOrDefault(x => _mediaTypes.Any(a => x.MediaType.ToLowerInvariant().Contains(a)));

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

        protected virtual bool NeedsHeaderVersioning(HttpRequestMessage request, out string version)
        {
            if (!string.IsNullOrWhiteSpace(_apiVersionHeader) && request.Headers.Contains(_apiVersionHeader))
            {
                version = request.Headers.GetValues(_apiVersionHeader).FirstOrDefault();
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

        public virtual int GetVersionFromRequest(HttpRequestMessage request)
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