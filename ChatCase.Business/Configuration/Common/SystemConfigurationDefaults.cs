using ChatCase.Core.Caching;
using ChatCase.Core.Domain.Configuration;

namespace ChatCase.Business.Configuration.Common
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class SystemConfigurationDefaults
    {

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("Chatcase.setting.all.dictionary.", EntityCacheDefaults<Setting>.Prefix);

        #endregion

        /// <summary>
        /// Gets the path to file that contains app settings
        /// </summary>
        public static string AppConfigsFilePath => "appsettings.json";
    }
}
