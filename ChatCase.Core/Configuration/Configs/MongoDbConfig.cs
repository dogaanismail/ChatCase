namespace ChatCase.Core.Configuration.Configs
{
    /// <summary>
    /// Represents mongoDB configuration parameters
    /// </summary>
    public partial class MongoDbConfig : IConfig
    {
        /// <summary>
        /// Gets or sets the default mongoDB connectionString default
        /// </summary>
        public string ConnectionString { get; set; } = "databaseconnection-string";

        /// <summary>
        /// Gets or sets the default mongoDB database default
        /// </summary>
        public string Database { get; set; } = "database";
    }
}
