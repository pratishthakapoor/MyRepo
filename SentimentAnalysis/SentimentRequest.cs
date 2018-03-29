using System;
using System.Collections.Generic;

namespace ProactiveBot.SentimentAnalysis
{
    public class SentimentRequest : TextRequest
    {
        #region Constructors

        /// <summary>
        /// Request for interacting with the text analytics sentiment analysis API
        /// </summary>

        public SentimentRequest()
        {
            this.Documents = new List<IDocument>();
            this.ValidLanguages = new List<string>() { "en", "es", "fr", "pt", "da", "de", "el", "fi", "it", "nl", "no", "pl", "ru", "sv", "tr" };
        }

        #endregion Constructors

        #region Properties

        public List<string> ValidLanguages
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override void Validate()
        {
            base.Validate();

            if (this.ValidLanguages != null && this.ValidLanguages.Count > 0)
                {
                   foreach(var document in this.Documents)
                    {
                      var sentimentDocumnet = document as SentimentDocument;
                      if (!string.IsNullOrEmpty(sentimentDocumnet.Language))
                      {
                        if (!this.ValidLanguages.Contains(sentimentDocumnet.Language))
                            throw new LanguageNotSupportedException(sentimentDocumnet.Language, this.ValidLanguages);
                      }
                    }
                }
                   
        }

        #endregion Methods

    }
}