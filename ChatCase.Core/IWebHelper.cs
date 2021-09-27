namespace ChatCase.Core
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public interface IWebHelper
    {
        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Get this page url
        /// </summary>
        /// <param name="includeQueryString"></param>
        /// <param name="useSsl"></param>
        /// <param name="lowercaseUrl"></param>
        /// <returns></returns>
        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();
    }
}
