using Newtonsoft.Json;

namespace ProactiveBot.SentimentAnalysis
{
    /// <summary>
    /// Error returned by Text Analytics API for a document
    /// </summary>
    public class DocumentError
    {
        #region Properties 

        /// <summary>
        /// Gets or sets the identifier of the document containing the error
        /// </summary>
        /// <value>
        /// Identifier of the document   
        /// </value>

        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }
        [JsonProperty("message")]
        public string Message
        {
            get;
            set;
        }

        #endregion Properties
    }
}