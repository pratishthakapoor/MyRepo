using Newtonsoft.Json;

namespace ProactiveBot.SentimentAnalysis
{
    internal class SentimentDocument : Document, IDocument
    {

        //public string Id { get; internal set; }
        //public sentence Text { get; internal set; }

        #region Properties

        /// <summary>
        /// Gets or sets the language the text is in
        /// </summary>
        /// <value>
        /// The Language
        /// </value>
        [JsonProperty("language")]
        public string Language { get; set; }

        #endregion Properties
    }
}