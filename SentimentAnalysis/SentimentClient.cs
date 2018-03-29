using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ProactiveBot.SentimentAnalysis
{
    public class SentimentClient : TextClient
    {
        #region Constructors

        /// <summary>
        /// Intializes a new Instance of the SentimentCVlient class
        /// </summary>
        /// <param name="apiKey"></param>

        public SentimentClient(string apiKey) : base(apiKey)
        {
            // URL for the sentiment dectection and scoring it on the index of 0 or 1

            this.Url = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the sentiment for a collection of documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns a SentimentResponse object from the Sentiment Ana</returns>

        public SentimentResponse GetSentiment(SentimentRequest request)
        {
            return GetSentimentAsync(request).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<SentimentResponse> GetSentimentAsync(SentimentRequest request)
        {
            request.Validate();

            var url = this.Url;

            var json = JsonConvert.SerializeObject(request);
            var responseJson = await this.SendPostAsync(url, json);
            var response = JsonConvert.DeserializeObject<SentimentResponse>(responseJson);

            return response;
        }

        #endregion Methods
    }
}