using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProactiveBot.SentimentAnalysis
{
   public abstract class TextRequest
    {
        #region Constructors

        /// <summary>
        /// Intializes the new instance of the class
        /// </summary>

        public TextRequest()
        {
            this.Documents = new List<IDocument>();

        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the document associated with the request
        /// </summary>
        /// <value>
        /// The documents associated with the request.
        /// </value>

        [JsonProperty("documents")]
        public virtual List<IDocument> Documents
        {
            get;
            set;
        }
       
        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates the request
        /// </summary>

        public virtual void Validate()
        {
            //must have at leats one document
            if(this.Documents == null || this.Documents.Count <= 0)
            {
                throw new DocumentCollectionMinDocumentException(0, 1);
            }

            if(this.Documents.Count >1000)
            {
                throw new DocumentCollectionMaxDocumentException(this.Documents.Count, 1000);
            }

            var collectionSize = 0;
            var documentIds = new List<string>();

            foreach(var document in this.Documents)
            {
                //document must have an id
                if(string.IsNullOrWhiteSpace(document.Id))
                {
                    throw new DocumentIdRequiredException();
                }
                else
                {
                    documentIds.Add(document.Id);
                }
                // documnet szie must be greater than 0 and less than or equal to 10kb

                if(document.Size <= 0 )
                {
                    throw new DocumentMinSizeException(document.Id, document.Size, 1);
                }

                if(document.Size > 10240)
                {
                    throw new DocumentMaxSizeException(document.Id, document.Size, 10240);
                }

                collectionSize = collectionSize + document.Size;
            }
           
            if(collectionSize > 1048576)
            {
                throw new DocumentCollectionMaxSizeException(collectionSize, 1045876);
            }

        }

        #endregion
    }
}