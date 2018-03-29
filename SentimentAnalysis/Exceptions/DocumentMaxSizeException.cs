using System;
using System.Runtime.Serialization;

namespace ProactiveBot.SentimentAnalysis
{
    [Serializable]
    internal class DocumentMaxSizeException : Exception
    {

        #region Constructor

        public DocumentMaxSizeException()
        {
        }

        public DocumentMaxSizeException(string message) : base(message)
        {
        }

        public DocumentMaxSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Intializes a new instance of the DocumentMaxSizeException class
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="documentSize"></param>
        /// <param name="maximumDocumentSize"></param>

        public DocumentMaxSizeException(string documentId, int documentSize, int maximumDocumentSize)
        {
            this.DocumentId = documentId;
            this.DocumentSize = documentSize;
            this.MaximumDocumentSize = maximumDocumentSize;
        }

        protected DocumentMaxSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the document identifier
        /// </summary>
        /// <value>
        /// The document identifier
        /// </value>

        public object DocumentId
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

        public object DocumentSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum size of the documents
        /// </summary>
        /// <value>
        /// The maximum size of the document
        /// </value>

        public object MaximumDocumentSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a message that describes the current Exception
        /// </summary>

        public override string Message
        {
            get
            {
                return string.Format("Document {0} is {1} bytes. Documents have maximum size of {2} bytes.", DocumentId, DocumentSize, MaximumDocumentSize);
            }
        }

        #endregion

    }
}