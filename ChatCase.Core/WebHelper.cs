using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;

namespace ChatCase.Core
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public class WebHelper : IWebHelper
    {
        #region Fields 

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Ctor

        public WebHelper(IActionContextAccessor actionContextAccessor,
            IHostApplicationLifetime hostApplicationLifetime,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _hostApplicationLifetime = hostApplicationLifetime;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsRequestAvailable()
        {
            if (_httpContextAccessor?.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Is IP address specified
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected virtual bool IsIpAddressSet(IPAddress address)
        {
            var rez = address != null && address.ToString() != IPAddress.IPv6Loopback.ToString();

            return rez;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(result) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    result = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            if (result != null && result.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                result = IPAddress.Loopback.ToString();

            if (IPAddress.TryParse(result ?? string.Empty, out var ip))
                result = ip.ToString();

            else if (!string.IsNullOrEmpty(result))
                result = result.Split(':').FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString"></param>
        /// <param name="useSsl"></param>
        /// <param name="lowercaseUrl"></param>
        /// <returns></returns>
        public string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var storeLocation = GetStoreLocation(useSsl ?? IsCurrentConnectionSecured());

            var pageUrl = $"{storeLocation.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

            if (includeQueryString)
                pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

            if (lowercaseUrl)
                pageUrl = pageUrl.ToLowerInvariant();

            return pageUrl;
        }

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns></returns>
        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        /// <summary>
        ///  Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl"></param>
        /// <returns></returns>
        public virtual string GetStoreLocation(bool? useSsl = null)
        {
            var storeLocation = string.Empty;

            var storeHost = GetStoreHost(useSsl ?? IsCurrentConnectionSecured());

            if (!string.IsNullOrEmpty(storeHost))
                storeLocation = IsRequestAvailable() ? $"{storeHost.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.PathBase}" : storeHost;

            storeLocation = $"{storeLocation.TrimEnd('/')}/";

            return storeLocation;
        }

        /// <summary>
        ///  Gets store host location
        /// </summary>
        /// <param name="useSsl"></param>
        /// <returns></returns>
        public virtual string GetStoreHost(bool useSsl)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (StringValues.IsNullOrEmpty(hostHeader))
                return string.Empty;

            var storeHost = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader.FirstOrDefault()}";

            storeHost = $"{storeHost.TrimEnd('/')}/";

            return storeHost;
        }

        #endregion
    }
}
