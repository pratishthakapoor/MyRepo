using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProactiveBot.SentimentAnalysis
{
    [Serializable]
    internal class LanguageNotSupportedException : Exception
    {
        #region Constructor

        public LanguageNotSupportedException()
        {
        }

        public LanguageNotSupportedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Intializes a new instance of the LanguageNotSupportedException class
        /// </summary>
        /// <param name="invalidLanguage">The invalid languages</param>
        /// <param name="validLanguages">The valid languages</param>

        public LanguageNotSupportedException(string invalidLanguage, List<string> validLanguages)
        {
            this.InvalidLanguage = invalidLanguage;
            this.ValidLanguages = validLanguages;
        }

        public LanguageNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LanguageNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructors

        #region Properties

        public string InvalidLanguage
        {
            get;
            set;
        }
        public List<string> ValidLanguages
        {
            get;
            set;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("Language {0} is not supported. Supported languages are:"));

                foreach (var language in this.ValidLanguages)
                    sb.AppendLine(language);

                return sb.ToString();
            }
        }

        #endregion
    }
}