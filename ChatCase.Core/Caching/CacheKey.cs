using ChatCase.Core.Configuration.Configs;
using ChatCase.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatCase.Core.Caching
{
    /// <summary>
    /// Represents key for caching objects
    /// </summary>
    public partial class CacheKey
    {
        #region Ctor

        /// <summary>
        /// Initialize a new instance with key and prefixes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefixes"></param>
        public CacheKey(string key, params string[] prefixes)
        {
            Key = key;
            Prefixes.AddRange(prefixes.Where(prefix => !string.IsNullOrEmpty(prefix)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new instance from the current one and fill it with passed parameters
        /// </summary>
        /// <param name="createCacheKeyParameters"></param>
        /// <param name="keyObjects"></param>
        /// <returns></returns>
        public virtual CacheKey Create(Func<object, object> createCacheKeyParameters, params object[] keyObjects)
        {
            var cacheKey = new CacheKey(Key, Prefixes.ToArray());

            if (!keyObjects.Any())
                return cacheKey;

            cacheKey.Key = string.Format(cacheKey.Key, keyObjects.Select(createCacheKeyParameters).ToArray());

            for (var i = 0; i < cacheKey.Prefixes.Count; i++)
                cacheKey.Prefixes[i] = string.Format(cacheKey.Prefixes[i], keyObjects.Select(createCacheKeyParameters).ToArray());

            return cacheKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a cache key
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets or sets prefixes for remove by prefix functionality
        /// </summary>
        public List<string> Prefixes { get; protected set; } = new List<string>();

        /// <summary>
        /// Gets or sets a cache time in minutes
        /// </summary>
        public int CacheTime { get; set; } = Singleton<AppConfig>.Instance.CacheConfig.DefaultCacheTime;

        #endregion
    }
}
