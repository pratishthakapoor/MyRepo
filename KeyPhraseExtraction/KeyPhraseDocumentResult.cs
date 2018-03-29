using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProactiveBot.KeyPhraseExtraction
{
    public class KeyPhraseDocumentResult
    {
        #region Properties 

        /// <summary>
        /// Gets or sets the identifier of the document
        /// </summary>
        /// <value>
        /// The identifier
        /// </value>

        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("keyPhrases")]
        public List<string> KeyPhrases
        {
            get;
            set;
        }


        #endregion Properties

    }
}