using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace ProactiveBot.SentimentAnalysis
{
    /// <summary>
    /// Document to submit to the Text Analytics API for analysis
    /// </summary>

    public class Document : IDocument
    {
        #region Properties
        /// <summary>
        /// Gets or sets the documnent's identifier.
        /// </summary>
        /// <value>
        /// The identifier of the documents
        /// </value>
        /// 
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the size of the documents in bytes
        /// </summary>
        /// <value>
        /// The size of the document in bytes
        /// </value>

        [JsonIgnore]
        public int Size
        {
            get
            {
                if(String.IsNullOrWhiteSpace(this.Text))
                {
                    return 0;
                }
                else
                {
                    return Encoding.UTF8.GetByteCount(this.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Text of the document.
        /// </summary>
        /// <value>
        /// The text of the document
        /// </value>
        [JsonProperty("text")]
        public string Text
        {
            get;
            set;
        }

        #endregion Properties'

        #region Methods
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            this.Text = this.Text.Replace("\"", "");
        }

        public override string ToString()
        {
            return Text;
        }

        #endregion Methods
    }
}