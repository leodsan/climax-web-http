using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Climax.Web.Http.Results
{
    public class FileHttpActionResult : IHttpActionResult
    {
        private readonly string _mediaType;
        public string FilePath { get; private set; }

        public FileHttpActionResult(string filePath, string mediaType)
        {
            _mediaType = mediaType;
            FilePath = filePath;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(FilePath))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);
            return Task.FromResult(result);
        }
    }
}
