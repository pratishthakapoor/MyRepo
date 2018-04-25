using System;
using System.Runtime.Serialization;

namespace ProactiveBot.SentimentAnalysis
{
    [Serializable]
    /// <summary>
    /// Exception thrown when maximum number of documents in the collections have exceeded
    /// </summary>

    internal class DocumentCollectionMaxDocumentException : Exception
    {

        #region Constructors 
        /// <summary>
        /// Intializes the new instance of the DocumentCollectionMaxDocumentException class
        /// </summary>

        public DocumentCollectionMaxDocumentException()
        {

        }

        public DocumentCollectionMaxDocumentException(string message) : base(message)
        {
        }

        /// <summary>
        /// Intializes the new instance of the DocumentCollectionMaxDocumentException class
        /// </summary>
        /// <param name="documentCount">The document count</param>
        /// <param name="maximumDocumentCount">The maximum document count allowed</param>

        public DocumentCollectionMaxDocumentException(int documentCount, int maximumDocumentCount)
        {
            this.DocumentCount = documentCount;
            this.MaximumDocumentCount = maximumDocumentCount;
        }

        public DocumentCollectionMaxDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentCollectionMaxDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the document count
        /// </summary>
        /// <value>
        /// The document count
        /// </value>
        public int DocumentCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the maximum document count
        /// </summary>
        /// <value>
        /// The document maximum count.
        /// </value>

        public int MaximumDocumentCount
        {
            get;
            set;
        }

        public override string Message
        {
            get
            {
                return string.Format("Documents collections has [0} documents. The maximum number of documents for a collection is {1}.", DocumentCount, MaximumDocumentCount);
            }
        }

        #endregion Properties
    }
}