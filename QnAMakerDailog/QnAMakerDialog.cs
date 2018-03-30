using Newtonsoft.Json;
using System;
using System.Net;

namespace QnAMakerDailog
{
    [Serializable]
    public class QnAMakerDialog
    {
        public QnAMakerResult GetQnAMakerResponse(string query, string subscriptionKey, string knowledgeBaseId)
        {
            var responseString = String.Empty;

            // Build the URI
            var builder = new UriBuilder($"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{knowledgeBaseId}/generateAnswer");

            //Add the question as part of the body

            var postBody = $"{{\"question\": \"{query}\"}}";

            // Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            //De-serilaize the response 
            QnAMakerResult response;
            try
            {
                var test = JsonConvert.DeserializeObject(responseString);
                response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
                return response;
            }
            catch (Exception)
            {
                throw new Exception("Unable to deserialize QnA Maker response string");
            }
        }
    }
        public class QnAMakerResult
        {
            public Answer[] answers { get; set; }

            public class Answer
            {
                [JsonProperty(PropertyName = "answer")]
                public string answer { get; set; }

                [JsonProperty(PropertyName ="score")]
                public double score { get; set;  }
            }
        }
}