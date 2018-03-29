using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.ConnectorEx;
using System.Net.Http;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.IO;  
using System.Web;  
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;
using ProactiveBot.SentimentAnalysis;
using ProactiveBot.KeyPhraseExtraction;
using System.Threading;

namespace Microsoft.Bot.Sample.ProactiveBot 
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]

    public class RaiseDialog : LuisDialog<object>
    {

        // Dictionary being used to show all the bot features to the user
        private readonly IDictionary<string, string> options = new Dictionary<string, string>
            {
                {"1", "Virtual Machine Setup"},
                {"2", "Raise Ticket"},
                {"3", "Raise Issue"},
                {"4", "Check Ticket Status"},
            };

        //None intent being called if user response is Invalid

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string Empty = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Null", out rec)) Empty = rec.Entity;
            if(string.IsNullOrEmpty(Empty))
            {
                await context.PostAsync("I didn't understand you");
            }
            else
            {
                await context.PostAsync($"Please enter a valid response");
            }
        }

        // Help intent implemented here to respond if user requires a help on the topic

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.HelpMessage);
            context.Wait(MessageReceived);
        }

        //Intent for handling the wishes response entered by the user

        [LuisIntent("Greetings")]
        public async Task Wishes(IDialogContext context, LuisResult result)
        {
            string Greetings = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Wishes", out rec)) Greetings = rec.Entity;

            if (string.IsNullOrEmpty(Greetings))
            {
                await context.PostAsync("I didn't understand you.");
            }
            else
            {
                await context.PostAsync($"{Greetings}, how can I assist you.");
                PromptDialog.Choice<string>(
                     context,
                     this.ChoiceRecievedAsync,
                     this.options.Values,
                     "Please select anyone of the options ",
                     "Oops, Some problem occured. Please try again.",
                     4,
                     PromptStyle.Auto,
                     this.options.Values
                    );
            }
        }

        //Method to hold the responses enterd by the user

        private async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result;
            Activity myActivity = (Activity)context.Activity;
            myActivity.Text = response.ToString();
            await MessageReceived(context, Awaitable.FromItem(myActivity));
        }
       
        // Intent to handle the Virtual Machine setup response

       [LuisIntent("Virtual Machine")]  
       public async Task VirtualMachine(IDialogContext context, LuisResult result)
        {
            string VirtualMachine = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("VirtualMachineOption", out rec)) VirtualMachine = rec.Entity;

            if(string.IsNullOrEmpty(VirtualMachine))
            {
                await context.PostAsync($"I didn't understand you.");
            }
            else
            {
                await context.PostAsync($"I would require some details to create a VM for you");
                context.Call(new FormDialog(), ChildDialogComplete);
            }
        }

        // Intent to handle the Raise ticket responses 

       [LuisIntent("Raise Ticket")]
       public async Task RaiseTicket(IDialogContext context, LuisResult result)
        {
            string response = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("RaiseTicketOption", out rec)) response = rec.Entity;

            if (string.IsNullOrEmpty(response))
            {
                await context.PostAsync($"I didn't understand you.");
            }
            else
            {
                await context.PostAsync($"I would require some information to {response} a ticket for you");
                try
            {
                var ticketForm = new FormDialog<TicketModel>(new TicketModel(), TicketModel.BuildForm, FormOptions.PromptInStart);
                context.Call(ticketForm, getKeyPhrases);
                //context.Call(ticketForm, getUserSentiment);

            }
            catch(Exception)
            {
                await context.PostAsync($"Some problem occured. You can try again later after some time.");
                context.Wait(MessageReceived);
            }
                
            }
        }

        // Intent to handle raise issue response given by the user

        [LuisIntent("Raise Issue")]
        public async Task RaiseIssue(IDialogContext context, LuisResult result)
        {
            string issue = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("ErrorOption", out rec)) issue = rec.Entity;

            if (string.IsNullOrEmpty(issue))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                await context.PostAsync($"Get back to you as soon as possible");
            }
        }

        //Luis Intent that handles the user negative responses
        [LuisIntent("NegResponse")]
        public async Task HandleNegResponse(IDialogContext context, LuisResult result)
        { 
            string negResponse = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("NegativeResponseOption", out rec)) negResponse = rec.Entity;

            if(string.IsNullOrEmpty(negResponse))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                await context.PostAsync($"Sorry, if you don't like my features.");
            }
        }

        // Intent to handle ticket status response input by the user

        [LuisIntent("TicketStatus")]
        public async Task CheckTicketStatus(IDialogContext context, LuisResult result)
        {
            string status = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("TicketStatusOption", out rec)) status = rec.Entity;

            if(string.IsNullOrEmpty(status))

            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                await context.PostAsync($"Checking the previous raised ticket status");
            }
        }

        //Intent to handle the user leaving responses input by the user 

        [LuisIntent("Appreciate")]
        public async Task LeavingResponses(IDialogContext context, LuisResult result)
        {
            string response = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("ComplimentOption", out rec)) response = rec.Entity;

            if(string.IsNullOrEmpty(response))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                await context.PostAsync($"I am happy that you liked our services. \r Do you require any further aasistance");
                PromptDialog.Confirm(context, ResumeAfterConfirmation, "What is your choice ?");
            }
        }

        // Method to handle the confirm Prompt Dialog

        private async Task ResumeAfterConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            await context.PostAsync(confirmation ? "Help required" : "Bye, hope to see you soon");

        }

        // Method to calculate the user sentiment score

        private async Task getUserSentiment(IDialogContext context, IAwaitable<TicketModel> result)
        {
            var sentence = await result;
            //string sentenceString = sentence.DatabaseName + "-" + sentence.MiddlewareName + "-" + sentence.ServerName;
            string sentenceString = sentence.Desc + "-" + sentence.DatabaseName + "-" + sentence.ServerName + "-" + sentence.MiddlewareName;
            var sentiment = await TextAnalyticsService.DetermineSentimentAsync(sentenceString);
            await context.PostAsync($"You rated our service as: {Math.Round(sentiment * 10, 1)}/10");

            if(sentiment< 0.5)
            {
                PromptDialog.Confirm(context, ResumeAfterFeedbackClarification,"I see it wasn't perfect, can we contact you about this?");
            }
        }

        // Method to extract the key phrases from the ticket model responses

        private async Task getKeyPhrases(IDialogContext context, IAwaitable<TicketModel> result)
        {
            var sentence = await result;
            string phraseString = sentence.Desc + "/" + sentence.ServerName + "/" + sentence.MiddlewareName + "/" + sentence.DatabaseName;
            var phrases = await KeyPhraseAnalytics.ExtractPhraseAsync(phraseString);
            string phrasesResult = String.Join(",", phrases.ToArray());
            await context.PostAsync($"The key phrases extracted are: {phrasesResult}");

            phrasesResult.GetType();
            /*var cts = new CancellationTokenSource();
         
            await context.Forward(new RaiseDialog(), getUserSentiment, phrasesResult, cts.Token);*/
        }

        // Method to contact the user if the sentiment score is less than 0.5 index level

        private async Task ResumeAfterFeedbackClarification(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            await context.PostAsync(confirmation ? "We'll call you!" : "We won't contact you.");
            context.Done(this);
        }

        private Task TicketFormComplete(IDialogContext context, IAwaitable<TicketModel> result)
        {
            throw new NotImplementedException();
        }

        // ChildDialogComplete method is used on the completion of the Child Dialog genreated from the RaiseDialog Dialog

        private async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Thank you for visting us.");
            context.Done(this);
        }

        /*private async Task <double> getSentimentScore(string DocumentText)
        {
            string queryUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
            HttpClient client = new HttpClient()
            {
                DefaultRequestHeaders =
                {
                    {
                       "Ocp-Apim-Subscription-Key",
                       ""
                    },

                    {
                        "Accept",
                        "application/json"
                    }
                }
            };

            var textInput = new BatchInput
            {
                documents = new List<DocumentInput>
                {
                    new DocumentInput
                    {
                        Id = 1,
                        Text = DocumentText,
                    }
                }
            };

            var jsonInput = JsonConvert.SerializeObject(textInput);
            HttpResponseMessage postMessage;
            BatchResult response;
            try
            {
                postMessage = await client.PostAsync(queryUri, new StringContent(jsonInput, Encoding.UTF8, "application/json"));
                response = JsonConvert.DeserializeObject<BatchResult>(await postMessage.Content.ReadAsStringAsync());
            }
            catch(Exception e)
            {
                return 0.0;
            }
            return response?.documents[0]?.score ?? 0.0;
        }*/
    }
}