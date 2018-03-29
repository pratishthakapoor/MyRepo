using ProactiveBot.SentimentAnalysis;
using System;
using System.Collections.Generic;

namespace ProactiveBot.KeyPhraseExtraction
{
    public class KeyPhrasesRequest : TextRequest
    {

        #region Constructor

        /// <summary>
        /// Intializes the new instance of the KeyPhraseRequest class
        /// </summary>

        public KeyPhrasesRequest()
        {
            this.Documents = new List<IDocument>();
            this.ValidLanguages = new List<string>() { "en", "es", "de", "ja", "fr" };
        }

        #endregion Construcotr

        #region Properties

        /// <summary>
        /// Gets or sets the valid Languages. 
        /// </summary>
        /// <value>
        /// The valid languages.
        /// </value>

        public List<string> ValidLanguages
        {
            get;
            set;
        }

        #endregion Properties


    }
}