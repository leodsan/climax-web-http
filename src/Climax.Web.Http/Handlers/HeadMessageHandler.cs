using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Climax.Web.Http.Handlers
{
    public class HeadMessageHandler : DelegatingHandler
    {
        private const string Head = "IsHead";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Head)
            {
                request.Method = HttpMethod.Get;
                request.Properties.Add(Head, true);
            }

            var response = await base.SendAsync(request, cancellationToken);

            object isHead;
            response.RequestMessage.Properties.TryGetValue(Head, out isHead);

            if (isHead != null && ((bool)isHead))
            {
                var oldContent = await response.Content.ReadAsByteArrayAsync();
                var content = new StringContent(string.Empty);
                content.Headers.Clear();

                foreach (var header in response.Content.Headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }

                content.Headers.ContentLength = oldContent.Length;
                response.Content = content;
            }

            return response;
        }
    }
}
