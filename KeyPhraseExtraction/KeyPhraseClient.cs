using Newtonsoft.Json;
using ProactiveBot.SentimentAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProactiveBot.KeyPhraseExtraction
{
    public class KeyPhraseClient : TextClient
    {
        #region Constructor
        /// <summary>
        /// Intializes a new instance of the KeyPhraseClient class
        /// </summary>
        /// <param name="apiKey">The text Analytics API key</param>
        public KeyPhraseClient(string apiKey) : base(apiKey)
        {
            // URL for the Key Phrases extraction

            this.Url = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases";
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Gets the key phrases asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>

        public async Task<KeyPhrasesResponse> GetKeyPhrasesAsync(KeyPhrasesRequest request)
        {
            request.Validate();

            var url = this.Url;

            var json = JsonConvert.SerializeObject(request);
            var responseJson = await this.SendPostAsync(url, json);
            var response = JsonConvert.DeserializeObject<KeyPhrasesResponse>(responseJson);

            return response;
        }

        #endregion Methods
    }
}