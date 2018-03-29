using System;
using System.Runtime.Serialization;

namespace ProactiveBot.SentimentAnalysis
{
    [Serializable]
    internal class DocumentMinSizeException : Exception
    {
        #region Constructor

        public DocumentMinSizeException()
        {
        }

        public DocumentMinSizeException(string message) : base(message)
        {
        }

        public DocumentMinSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Intializes the instance of the DocumentMinSizeException class
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="documentSize"></param>
        /// <param name="minimumSizeDocument"></param>

        public DocumentMinSizeException(string documentId, int documentSize, int minimumSizeDocument)
        {
            this.DocumentId = documentId;
            this.DocumentSize = documentSize;
            this.MinimumSizeDocument = minimumSizeDocument;
        }

        protected DocumentMinSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the document ientifier
        /// </summary>
        /// <value>
        /// The document identifier
        /// </value> 

        public string DocumentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the document
        /// </summary>
        /// <value>
        /// The size of the document
        /// </value>

        public int DocumentSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets  the minimum size of the document
        /// </summary>
        /// <value>
        /// The minimum size of the document
        /// </value>

        public int MinimumSizeDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a message that describes the current exception
        /// </summary>

        public override string Message
        {
            get
            {
                return string.Format("Document {0} is {1} bytes. Documents have a minimum size of {2} bytes.", DocumentId, DocumentSize, MinimumSizeDocument);
            }
        }
        #endregion
    }
}