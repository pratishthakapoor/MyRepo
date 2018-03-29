using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProactiveBot.SentimentAnalysis
{
    public abstract class TextClient
    {
        #region Fields
        private const string APPLICATION_JSON_CONTENT_TYPE = "application/json";
        private const string GET_METHOD = "GET";
        private const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
        private const string POST_METHOD = "POST";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Intializes the content of TextClient class
        /// </summary>
        /// <param name="apiKey"></param>
        public TextClient(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the Text Analytics API key
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Text Analytics Service URL
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Send the post to the Text Analytics API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        protected string SendPost(string data)
        {
            return SendPost(this.Url, data);
        }

        /// <summary>
        /// Send the post to the Text Analytics API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        private string SendPost(string url, string data)
        {
            return this.SendPostAsync(url, data).Result;
        }

        /// <summary>
        /// Send the post to the Text Analytics API asynchronously 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        protected async Task<string> SendPostAsync(string data)
        {
            return await SendPostAsync(this.Url, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        protected async Task<string> SendPostAsync(string url, string data)
        {
            if(String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException(nameof(url));
            }

            if(String.IsNullOrWhiteSpace(this.ApiKey))
            {
                throw new ArgumentException(nameof(ApiKey));
            }

            if(String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException(nameof(data));
            }

            var responseData = "";

            using (var client = new HttpClient { BaseAddress = new Uri(url) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(APPLICATION_JSON_CONTENT_TYPE));
                client.DefaultRequestHeaders.Add(OCP_APIM_SUBSCRIPTION_KEY, ApiKey);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                using (var response = await client.PostAsync(url, content))
                {
                    responseData = await response.Content.ReadAsStringAsync();
                }
            }
                return responseData;
        }

        /// <summary>
        /// Sends the get to the Text Analytics API
        /// </summary>
        /// <returns></returns>

        protected string SendGet()
        {
            return SendGet(this.Url);
        }

        /// <summary>
        /// Sends the get to the Text Analytic API
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>

        protected string SendGet(string url)
        {
            return SendGetAync(url).Result;
        }

        /// <summary>
        /// Send the get to the Text Abnalytics API asynchronously
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        protected async Task<string> SendGetAync()
        {
            return await SendGetAync(this.Url);
        }

        protected async Task<string> SendGetAync(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException(nameof(url));
            }

            if (String.IsNullOrWhiteSpace(this.ApiKey))
            {
                throw new ArgumentException(nameof(ApiKey));
            }

            var responseData = "";

            using (var client = new HttpClient { BaseAddress = new Uri(url) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(APPLICATION_JSON_CONTENT_TYPE));
                client.DefaultRequestHeaders.Add(OCP_APIM_SUBSCRIPTION_KEY, ApiKey);

                using (var response = await client.GetAsync(url))
                {
                    responseData = await response.Content.ReadAsStringAsync();
                }
            }

            return responseData;
        }

        #endregion Methods
    }
}