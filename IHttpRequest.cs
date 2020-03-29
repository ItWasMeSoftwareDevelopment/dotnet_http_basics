using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;  

namespace ItWasMe.HttpBasics
{
    public interface IHttpRequest
    {
        string GetCookieValue(Uri uri, string name);
        IHttpRequest SetReferer(Uri referer);
        IHttpRequest SetTimeout(TimeSpan timeout);
        IHttpRequest SetCookie(Uri uri, params Cookie[] cookies);
        IHttpRequest SetHeaders(string key, string value);
        IHttpRequest SetAuthorizationCookies(Uri uri, IHaveAuthorizationCookies session);
        IHttpRequest SetProxy(IProxyModel proxyModel);
        IHttpRequest SetUserAgent(string value);
        IHttpRequest SetLanguage(string value);
        IHttpRequest SetConnection(string value);
        IHttpRequest TryAddHeader(string key, string value);
        Task<HttpResponseMessage> GetAsync(string requestUri);
        //Task<HttpRequestIpDetails> GetIpDetailsAsync();
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content);
        Task<byte[]> GetByteArrayAsync(string requestUri);
        IReadOnlyCollection<Cookie> GetAllCookie(Uri uri);
    }
}
