using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ChatCase.Core.Configuration.Configs
{
    /// <summary>
    /// Represents the app configs
    /// </summary>
    public partial class AppConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets cache configuration parameters
        /// </summary>
        public CacheConfig CacheConfig { get; set; } = new CacheConfig();

        /// <summary>
        /// Gets or sets distributed cache configuration parameters
        /// </summary>
        public DistributedCacheConfig DistributedCacheConfig { get; set; } = new DistributedCacheConfig();

        /// <summary>
        /// Gets or sets distributed cache configuration parameters
        /// </summary>
        public MongoDbConfig MongoDbConfig { get; set; } = new MongoDbConfig();

        /// <summary>
        /// Gets or sets common configuration parameters
        /// </summary>
        public CommonConfig CommonConfig { get; set; } = new CommonConfig();

        /// <summary>
        /// Gets or sets jwt configuration parameters
        /// </summary>
        public JwtConfig JwtConfig { get; set; } = new JwtConfig();

        /// <summary>
        /// Gets or sets additional configuration parameters
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }

        #endregion
    }
}
