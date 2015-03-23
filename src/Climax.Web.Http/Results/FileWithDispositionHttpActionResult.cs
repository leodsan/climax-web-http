using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Climax.Web.Http.Results
{
    public class FileWithDispositionHttpActionResult : IHttpActionResult
    {
        private readonly string _fileName;
        private readonly string _mediaType;
        private readonly DispositionType _dispositionType;
        public string FilePath { get; private set; }        

        public FileWithDispositionHttpActionResult(string filePath, string fileName, string mediaType, DispositionType dispositionType)
        {
            _fileName = fileName;
            _mediaType = mediaType;
            _dispositionType = dispositionType;
            FilePath = filePath;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(FilePath))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var fileBytes = File.ReadAllBytes(FilePath);
            result.Content = new ByteArrayContent(fileBytes);
            result.Headers.ConnectionClose = false;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(_dispositionType.ToString().ToLowerInvariant())
            {
                FileName = _fileName
            };
            return Task.FromResult(result);
        }
    }
}