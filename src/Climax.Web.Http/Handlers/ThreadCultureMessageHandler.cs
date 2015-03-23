using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Climax.Web.Http.Handlers
{
    public class ThreadCultureMessageHandler : DelegatingHandler
    {
        private readonly CultureInfo _lastResortFallback;
        private readonly bool _setThreadCulture;
        private readonly bool _setThreadUiCulture;

        public ThreadCultureMessageHandler(CultureInfo fallBackCulture) : this(true, true, fallBackCulture) {}

        public ThreadCultureMessageHandler(bool setThreadCulture, bool setThreadUiCulture, CultureInfo fallBackCulture)
        {
            _setThreadCulture = setThreadCulture;
            _setThreadUiCulture = setThreadUiCulture;
            _lastResortFallback = fallBackCulture ?? new CultureInfo("en-CA");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // the fall back language
            var langauges = new List<StringWithQualityHeaderValue>();

            // Check the AcceptLanguage simulated header in the query string first. This will ignore lettercase.
            NameValueCollection query = request.GetQueryNameValuePairs()
                .Aggregate(new NameValueCollection(), (col, kvp) =>
                {
                    col.Add(kvp.Key, kvp.Value);
                    return col;
                });

            if (query[FallbackHeaderConstants.AcceptLanguage] != null)
            {
                langauges.Add(new StringWithQualityHeaderValue(query[FallbackHeaderConstants.AcceptLanguage], 1.0));
            }
            else if (request.Headers.AcceptLanguage != null)
            {
                // then check the Accept-Language header.
                langauges.AddRange(request.Headers.AcceptLanguage);
            }

            langauges = langauges.OrderByDescending(l => l.Quality).ToList();

            // this is the final fall-back culture.
            var culture = CultureInfo.DefaultThreadCurrentCulture;
            var uiCulture = CultureInfo.DefaultThreadCurrentUICulture;

            // try to break and find one language that's available
            foreach (StringWithQualityHeaderValue lang in langauges)
            {
                try
                {
                    culture = CultureInfo.GetCultureInfo(lang.Value);
                    uiCulture = culture;
                    break;
                }
                catch (CultureNotFoundException)
                {
                    // ignore the error
                }
            }

            if (_setThreadCulture)
            {
                Thread.CurrentThread.CurrentCulture = culture ?? _lastResortFallback;
            }
            if (_setThreadUiCulture)
            {
                Thread.CurrentThread.CurrentUICulture = uiCulture ?? _lastResortFallback;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}