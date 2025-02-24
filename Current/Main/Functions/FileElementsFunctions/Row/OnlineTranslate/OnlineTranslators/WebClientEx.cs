using System;
using System.Net;
using System.Net.Cache;

namespace TranslationHelper.OnlineTranslators
{
    //https://stackoverflow.com/a/11523836
    public class WebClientEx : WebClient
    {
        public WebClientEx(CookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
        }

        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest httpWebRequest)
                httpWebRequest.CookieContainer = CookieContainer;

            return webRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest webRequest)
        {
            var webResponse = base.GetWebResponse(webRequest);
            ReadCookies(webResponse);

            return webResponse;
        }

        private void ReadCookies(WebResponse webResponse)
        {
            if (webResponse is HttpWebResponse httpWebResponse)
            {
                CookieCollection cookies = httpWebResponse.Cookies;
                CookieContainer.Add(cookies);
            }
        }
    }
}
