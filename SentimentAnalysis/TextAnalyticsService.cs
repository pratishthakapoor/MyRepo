using Microsoft.Bot.Sample.ProactiveBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProactiveBot.SentimentAnalysis
{
    public class TextAnalyticsService
    {
        private static string TEXT_ANALYTICS_KEY = Constants.TEXT_ANALYTICS_ID;

        private static SentimentClient _client = new SentimentClient(TEXT_ANALYTICS_KEY);

        public static async Task <float> DetermineSentimentAsync(string sentence)
        {
            var request = new SentimentRequest();
            request.Documents.Add(new SentimentDocument()
            {
                Id = Guid.NewGuid().ToString(),
                Text = sentence,
                Language = "en"
            });

            var result = await _client.GetSentimentAsync(request);

            return result.Documents.First().Score;

            //return 1;
        }
    }
}