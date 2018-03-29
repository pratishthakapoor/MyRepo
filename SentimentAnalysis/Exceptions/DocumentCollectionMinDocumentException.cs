using System;
using System.Runtime.Serialization;

namespace ProactiveBot.SentimentAnalysis
{
    [Serializable]

    internal class DocumentCollectionMinDocumentException : Exception
    {
        private int v1;
        private int v2;

        #region Constructor

        public DocumentCollectionMinDocumentException()
        {
        }

        public DocumentCollectionMinDocumentException(string message) : base(message)
        {
        }


        /// <summary>
        /// Intializes the new instance of the DocumentCollectionMinDocumentException class.
        /// </summary>
        /// <param name="documentCount">The dcount of the documnets in the collection</param>
        /// <param name="MinimumDocumentCount">The mimimum count of the documents in the collection</param>

        public DocumentCollectionMinDocumentException(int documentCount, int MinimumDocumentCount)
        {
            this.DocumentCount = documentCount;
            this.MinimumDocumentCount = MinimumDocumentCount;
        }

        public DocumentCollectionMinDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentCollectionMinDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the count of the documents in the collection
        /// </summary>
        /// <value>
        /// Tthe document count
        /// </value>

        public int DocumentCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum number of documents in a collection
        /// </summary>
        /// <value>
        /// The minimum document count
        /// </value>

        public int MinimumDocumentCount
        {
            get;
            set;
        }

        public override string Message
        {
            get
            {
                return string.Format("Document collection has {0} documents.The minimum number of documents for a collection is {1}.", DocumentCount, MinimumDocumentCount);

            }
        }

        #endregion Properties

    }
}