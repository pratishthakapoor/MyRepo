using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProactiveBot.SentimentAnalysis
{
    public class SentimentResponse
    {
        #region Constructor
        /// <summary>
        /// Intializes a new instance of the SentimentTResponse class
        /// </summary>

        public SentimentResponse()
        {
            this.Documents = new List<SentimentDocumnetResult>();
            this.Errors = new List<DocumentError>();
        }
        #endregion Constructor

        #region Properties
        
        [JsonProperty("documents")]
        public List<SentimentDocumnetResult> Documents
        {
            get;
            set;
        }

        [JsonProperty("errors")]
        public List<DocumentError> Errors
        {
            get;
            set;
        }

        #endregion Properties
    }
}