namespace ProactiveBot.SentimentAnalysis
{
    public interface IDocument
    {
        /// <summary>
        /// Gets and sets the identifier
        /// </summary>
        /// <value>
        /// The identifier
        /// </value>
        string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the size 
        /// <value>
        /// The size.
        /// </value>
        /// </summary>

        int Size
        {
            get;
        }

        /// <summary>
        /// gets and sets the text
        /// </summary>
        /// <value>
        /// The Text
        /// </value>

        string Text
        {
            get;
            set;
        }
    }
}