using System.Net.Http;

namespace Climax.Web.Http.Services.Versioning
{
    public interface IVersionParser
    {
        int GetVersionFromRequest(HttpRequestMessage request);
    }
}