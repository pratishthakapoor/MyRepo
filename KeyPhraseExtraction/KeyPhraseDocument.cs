using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using ProactiveBot.SentimentAnalysis;

namespace ProactiveBot.KeyPhraseExtraction
{
    public class KeyPhraseDocument : SentimentAnalysis.Document, IDocument
    {
        /// <summary>
        /// Gets or sets the language the text is in
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [JsonProperty("language")]
        public string Language
        {
            get;
            set;
        }
    }
}