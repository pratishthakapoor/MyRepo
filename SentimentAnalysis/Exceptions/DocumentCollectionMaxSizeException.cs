using System;
using System.Runtime.Serialization;

namespace ProactiveBot.SentimentAnalysis
{
    /// <summary>
    /// Exception thrown when the maximum size of the document collection is exceeded
    /// </summary>

    [Serializable]
    internal class DocumentCollectionMaxSizeException : Exception
    {
       

        #region Constructors

        public DocumentCollectionMaxSizeException()
        {
        }

        public DocumentCollectionMaxSizeException(string message) : base(message)
        {
        }


        /// <summary>
        /// Intializes a new instance of the  DocumentCollectionMaxSizeException class.
        /// </summary>
        /// <param name="collectionSize">Size of the document collection</param>
        /// <param name="maximumCollectionSize">Maximum size of the document collection</param>
        public DocumentCollectionMaxSizeException(int collectionSize, int maximumCollectionSize)
        {
            this.CollectionSize = collectionSize;
            this.MaximumCollectionSize = maximumCollectionSize;
        }

        public DocumentCollectionMaxSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentCollectionMaxSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the size of the document collection
        /// </summary>
        /// <value>
        /// Size of the document collection
        /// </value>

        public int CollectionSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the maximum document collection
        /// </summary>
        /// <value>
        /// The maximum size of the collection.
        /// </value>

        public int MaximumCollectionSize
        {
            get;
            set;
        }

  
        public override string Message
        {
            get
            {
                return string.Format("Document collection is {0} bytes. Document collections have maximum size of {1} bytes.", CollectionSize, MaximumCollectionSize);
            }
        }

        #endregion Properties

    }
}