using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProactiveBot.SentimentAnalysis
{
    public class SentimentDocumnetResult
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

        [JsonProperty("score")]
        public float Score
        {
            get;
            set;
        }

        #endregion
    }
}