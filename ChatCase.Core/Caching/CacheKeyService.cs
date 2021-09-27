using ChatCase.Common.Helpers;
using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ChatCase.Core.Caching
{
    /// <summary>
    /// Represents the default cache key service implementation
    /// </summary>
    public abstract partial class CacheKeyService
    {
        #region Constants

        /// <summary>
        /// Gets an algorithm used to create the hash value of identifiers need to cache
        /// </summary>
        private static string HashAlgorithm => "SHA1";

        #endregion

        #region Fields

        protected readonly AppConfig _appConfig;

        #endregion

        #region Ctor

        protected CacheKeyService(AppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the cache key prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="prefixParameters"></param>
        /// <returns></returns>
        protected virtual string PrepareKeyPrefix(string prefix, params object[] prefixParameters)
        {
            return prefixParameters?.Any() ?? false
                ? string.Format(prefix, prefixParameters.Select(CreateCacheKeyParameters).ToArray())
                : prefix;
        }

        /// <summary>
        /// Create the hash value of the passed identifiers
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected virtual string CreateIdsHash(IEnumerable<string> ids)
        {
            var identifiers = ids.ToList();

            if (!identifiers.Any())
                return string.Empty;

            var identifiersString = string.Join(", ", identifiers.OrderBy(id => id));
            return HashHelper.CreateHash(Encoding.UTF8.GetBytes(identifiersString), HashAlgorithm);
        }

        /// <summary>
        /// Converts an object to cache parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual object CreateCacheKeyParameters(object parameter)
        {
            return parameter switch
            {
                null => "null",
                IEnumerable<string> ids => CreateIdsHash(ids),
                IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(entity => entity.Id)),
                BaseEntity entity => entity.Id,
                decimal param => param.ToString(CultureInfo.InvariantCulture),
                _ => parameter
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of cache key and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheKeyParameters"></param>
        /// <returns></returns>
        public virtual CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            return cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
        }

        /// <summary>
        /// Create a copy of cache key using the default cache time and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheKeyParameters"></param>
        /// <returns></returns>
        public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);

            key.CacheTime = _appConfig.CacheConfig.DefaultCacheTime;

            return key;
        }

        /// <summary>
        /// Create a copy of cache key using the short cache time and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheKeyParameters"></param>
        /// <returns></returns>
        public virtual CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);

            key.CacheTime = _appConfig.CacheConfig.ShortTermCacheTime;

            return key;
        }

        #endregion
    }
}
