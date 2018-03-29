using Newtonsoft.Json;
using ProactiveBot.SentimentAnalysis;
using System.Collections.Generic;

namespace ProactiveBot.KeyPhraseExtraction
{
    public class KeyPhrasesResponse
    {
        #region Constructors

        public KeyPhrasesResponse()
        {
            this.Documents = new List<KeyPhraseDocumentResult>();
            this.Errors = new List<DocumentError>();
        }

        #endregion Construcotr

        #region Properties

        /// <summary>
        /// Gets or sets the documents
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>

       [JsonProperty("documents")]
        public List<KeyPhraseDocumentResult> Documents { get; set; }

        /// <summary>
        /// Gets or sets the error
        /// </summary>
        /// <value>
        /// The errors
        /// </value>

        [JsonProperty("errors")]
        public List<DocumentError> Errors { get; set; }

        #endregion Properties
    }
}