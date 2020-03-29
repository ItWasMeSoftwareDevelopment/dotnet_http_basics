using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;  

namespace ItWasMe.HttpBasics
{
    public interface IHttpRequestBuilder
    {
        IHttpRequest Create();
    }

    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        public IHttpRequest Create()
        {
            return new HttpRequest();
        }

        private class HttpRequest : IHttpRequest
        {
            private const string IpDetailsUrl = "https://api.db-ip.com/v2/free/self";
            private static readonly Regex RegexStripHtmlTags = new Regex("<.*?>");
            private readonly ConcurrentDictionary<string, string> _headers;
            private readonly CookieContainer _cookieContainer;
            private string _userAgent;
            private Uri _referer;
            private string _connection;
            private string _acceptLanguage;
            private IProxyModel _proxyModel;
            private TimeSpan? _timeout;


            public HttpRequest()
            {
                _headers = new ConcurrentDictionary<string, string>();
                _cookieContainer = new CookieContainer();
            }

            public string GetCookieValue(Uri uri, string name)
            {
                return _cookieContainer.GetCookies(uri)
                    .Cast<Cookie>().FirstOrDefault(x => x.Name == name)?.Value;
            }

            public IReadOnlyCollection<Cookie> GetAllCookie(Uri uri)
            {
                return _cookieContainer.GetCookies(uri)
                    .Cast<Cookie>().ToList();
            }

            public IHttpRequest SetReferer(Uri referer)
            {
                _referer = referer;
                return this;
            }

            public IHttpRequest SetTimeout(TimeSpan timeout)
            {
                _timeout = timeout;
                return this;
            }

            public IHttpRequest TryAddHeader(string key, string value)
            {
                _headers.TryAdd(key, value);
                return this;
            }

            public IHttpRequest SetUserAgent(string value)
            {
                _userAgent = value;
                return this;
            }

            public IHttpRequest SetLanguage(string value)
            {
                _acceptLanguage = value;
                return this;
            }

            public IHttpRequest SetConnection(string value)
            {
                _connection = value;
                return this;
            }

            public IHttpRequest SetProxy(IProxyModel proxyModel)
            {
                _proxyModel = proxyModel;
                return this;
            }

            public IHttpRequest SetCookie(Uri uri, params Cookie[] cookies)
            {
                foreach (var cookie in cookies)
                {
                    _cookieContainer.Add(uri, cookie);
                }

                return this;
            }

            public IHttpRequest SetAuthorizationCookies(Uri uri, IHaveAuthorizationCookies session)
            {
                foreach (var cookie in session.AllCookies)
                {
                    _cookieContainer.Add(uri, cookie);
                }

                return this;
            }


            public IHttpRequest SetHeaders(string key, string value)
            {
                _headers.TryAdd(key, value);

                return this;
            }

            public async Task<HttpResponseMessage> GetAsync(string requestUri)
            {
                using (var httpClient = Create())
                {
                    SetHeaders(httpClient);
                    return await httpClient.GetAsync(requestUri);
                }
            }

            //public async Task<HttpRequestIpDetails> GetIpDetailsAsync()
            //{
            //    using (var httpClient = Create())
            //    {
            //        SetHeaders(httpClient);
            //        var result = await httpClient.GetStringAsync(IpDetailsUrl);

            //        var ipPage = StripHtmlTags(result);

            //        if (!ipPage.Contains("OVER_QUERY_LIMIT"))
            //        {
            //            var ipDetails =  JsonConvert.DeserializeObject<HttpRequestIpDetails>(ipPage);
            //            return new HttpRequestIpDetails()
            //            {
            //                CountryCode = ipDetails.CountryCode,
            //                City = ipDetails.City.NormalizeToForm(),
            //                ContinentCode = ipDetails.ContinentCode,
            //                ContinentName = ipDetails.ContinentName.NormalizeToForm(),
            //                CountryName = ipDetails.CountryName.NormalizeToForm(),
            //                IpAddress = ipDetails.IpAddress,
            //                State = ipDetails.State.NormalizeToForm()
            //            };
            //        }

            //        return null;
            //    }
            //}

            public async Task<byte[]> GetByteArrayAsync(string requestUri)
            {
                using (var httpClient = Create())
                {
                    SetHeaders(httpClient);
                    return await httpClient.GetByteArrayAsync(requestUri);
                }
            }

            public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
            {
                using (var httpClient = Create())
                {
                    SetHeaders(httpClient);
                    return await httpClient.PostAsync(uri, content);
                }
            }

            private HttpClient Create()
            {
                WebProxy proxy = null;
                if (_proxyModel != null)
                {
                    var proxyAddress = new Uri($"http://{_proxyModel.ProxyIp}:{_proxyModel.ProxyPort}");

                    proxy = new WebProxy()
                    {
                        Address = proxyAddress,
                    };

                    if (!string.IsNullOrWhiteSpace(_proxyModel.UserName) &&
                        !string.IsNullOrWhiteSpace(_proxyModel.Password))
                    {
                        proxy.Credentials = new NetworkCredential(_proxyModel.UserName, _proxyModel.Password);
                    }
                }

                var httpClient = new HttpClient(new HttpClientHandler
                {
                    Proxy = proxy,
                    CookieContainer = _cookieContainer,
                    UseDefaultCredentials = true,

                })
                {
                    Timeout = _timeout ?? TimeSpan.FromSeconds(10)
                };

                return httpClient;
            }

            private void SetHeaders(HttpClient client)
            {
                if (_headers != null)
                {
                    foreach (var kvp in _headers)
                    {
                        client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                    }
                }

                if (!string.IsNullOrWhiteSpace(_userAgent))
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
                if (_referer != null)
                    client.DefaultRequestHeaders.Referrer = _referer;
                if (!string.IsNullOrWhiteSpace(_acceptLanguage))
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(_acceptLanguage);
                if (!string.IsNullOrWhiteSpace(_connection))
                    client.DefaultRequestHeaders.Connection.ParseAdd(_connection);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.01));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/signed-exchange"));
            }
            private string StripHtmlTags(string input) => RegexStripHtmlTags.Replace(input, string.Empty);

        }
    }
}
