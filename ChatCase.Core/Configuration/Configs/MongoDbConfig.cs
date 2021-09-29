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
        public string ConnectionString { get; set; } = "mongodb+srv://basketcase2021:live2021basketcase@chatsystem.dwsth.mongodb.net/ChatCaseDb?retryWrites=true&w=majority";

        /// <summary>
        /// Gets or sets the default mongoDB database default
        /// </summary>
        public string Database { get; set; } = "ChatCaseDb";
    }
}
