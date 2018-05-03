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
using System.Linq;
using ProactiveBot.Dialogs;
using Microsoft.Bot.Sample.ProacticeBot;
using System.Data.SqlClient;
using System.Web.UI;

namespace Microsoft.Bot.Sample.ProactiveBot 
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]

    /**
     * Raise Dialog class works as the root dialog for this project 
     **/

    public class RaiseDialog : LuisDialog<object>
    {

        /**
         * Dictionary being used to show all the bot features to the user
         **/

        private readonly IDictionary<string, string> options = new Dictionary<string, string>
            {
                {"1", "Raise Ticket"},
                {"2", "Virtual Machine Configuration"},
                {"3", "Raise Issue"},
                {"4", "Check previous raised Ticket Status"},
                {"5", "Server Password Reset" },
            };


        /**
         * None intent being called if user response is Invalid
         **/

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

        /** Help intent implemented here to respond if user requires a help on the topic
         **/

        [LuisIntent("Help")]
        [LuisIntent("Question")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.HelpMessage);
            context.Wait(MessageReceived);
        }

        /**
         * Intent for handling the wishes response entered by the user
         */

        [LuisIntent("Greetings")]
        [LuisIntent("Name")]
        public async Task Wishes(IDialogContext context, LuisResult result)
        {
            string Greetings = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Wishes", out rec) || result.TryFindEntity("User", out rec)) Greetings = rec.Entity;

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

        /**
         * Method to hold the responses enterd by the user
         * */

        private async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result;
            Activity myActivity = (Activity)context.Activity;
            myActivity.Text = response.ToString();
            await MessageReceived(context, Awaitable.FromItem(myActivity));
        }
       
        /**
         * Intent to handle the Virtual Machine configuration response
         **/

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
                context.Call(new VMConfigDialog(), ChildDialogComplete);
            }
        }

        /**
         * Intent to handle the Raise ticket responses 
         **/

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
                string reply;
                await context.PostAsync($"I would require some information to {response} a ticket for you");
                try
                {
                    var ticketForm = new FormDialog<TicketModel>(new TicketModel(), TicketModel.BuildForm, FormOptions.PromptInStart);
                    //context.Call(ticketForm, getKeyPhrases);
                    context.Call(ticketForm, ChildDialogComplete);

                }
                catch(FormCanceledException<TicketModel> cancelled)
                {
                    if (cancelled.InnerException == null)
                        //await context.PostAsync($"You quit on {cancelled.Last}");
                        reply = $"You quit on {cancelled.Last} -- maybe you can finish next time!";
                    else
                        //await context.PostAsync($"Sorry, I have a problem here");
                        reply = $"Some problem occured. You can try again later after some time";
                    await context.PostAsync(reply);
                }
                catch (Exception)
                {
                    await context.PostAsync($"Some problem occured. You can try again later after some time.");
                    context.Wait(MessageReceived);
                }
            }
       }

        /**
         * Intent to handle raise issue response given by the user
         **/

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
                //await context.PostAsync($"Get back to you as soon as possible");
                //await context.PostAsync($"Just, reverting you there ");
                context.Call(new RaiseIssueDialog(), ChildDialogComplete);
            }
            //context.Done(this);
        }

        /**
         * Luis Intent that handles the user negative responses
         **/

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

        /**
         * Intent to handle ticket status response like appreciation and confirmation like ok, okay, hmm, fine
         **/

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
                context.Call(new GenerateStatusDialog(), ChildDialogComplete);
                //await context.PostAsync($"We couldn't find your previous request. Please reach out to us at FMB.FMB-FMB-TSINPresales@t-systems.com");
            }
        }

        /**
         * Intent to handle the user positive responses
         **/

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

        /**
         * Method to handle the password reset request for the Servers
         **/

        [LuisIntent("Reset Password")]
        public async Task ServerResetPassword(IDialogContext context, LuisResult result)
        {
            string resetPwd = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("ServerPwdOption", out rec)) resetPwd = rec.Entity;

            if(string.IsNullOrEmpty(resetPwd))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                context.Call(new ResetPasswordDialog(), ChildDialogComplete);
            }
        }

        /**
         * Method to handle the confirm Prompt Dialog
         * Checks wether user require a help or not
         **/

        private async Task ResumeAfterConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            await context.PostAsync(confirmation ? "Help required" : "Bye, hope to see you soon");
            /*var response = await result;
            Activity myActivity = (Activity)context.Activity;
            myActivity.Text = response.ToString();
            await MessageReceived(context, Awaitable.FromItem(myActivity));*/

        }

        // Method to extract the key phrases from the ticket model responses

        private async Task getKeyPhrases(IDialogContext context, IAwaitable<TicketModel> result)
        {
            var sentence = await result;
            string phraseString = sentence.Desc + "/" + sentence.ServerName + "/" + sentence.MiddlewareName + "/" + sentence.DatabaseName;
            var phrases = await KeyPhraseAnalytics.ExtractPhraseAsync(phraseString);
            string phraseResult = String.Join(",", phrases.ToArray());
            //string new_result = String.Concat(phraseString, phraseResult);

            await context.PostAsync($"The key phrases extracted are: {phraseResult},");

            var cts = new CancellationTokenSource();

            //System.Environment.Exit(0);

            //await context.Forward(new RaiseDialog(), getUserSentiment, result, cts.Token);
        }

        // Method to contact the user if the sentiment score is less than 0.5 index level

        private async Task ResumeAfterFeedbackClarification(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            await context.PostAsync(confirmation ? "We'll call you!" : "Thanks for your input, have a good day");
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


        // Method not been used in the class

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

        /**
         * Method tonhandle the interaction with the HP Service Manager - 9.0
         **/

        // method not used it is defined in the Ticket Model

        /**
         * Method to calculate the user sentiment score
         **/

        private async Task getUserSentiment(IDialogContext context, IAwaitable<TicketModel> result)
        {
            var sentence = await result;
            // string sentenceString = sentence.DatabaseName + "-" + sentence.MiddlewareName + "-" + sentence.ServerName;
            string sentenceString = sentence.Desc + "-" + sentence.DatabaseName + "-" + sentence.ServerName + "-" + sentence.MiddlewareName;
            //string sentenceString = sentence.Desc;

            /**
             * To call the GetQnAMakerResponse to get the responses to the user queries from QnA Maker KB
             * The QnA maker sends the appropriate response to the user queries 
             **/

            await context.PostAsync("Let me search my database for a solution to your problem");
            try
            {
                var activity = (Activity)context.MakeMessage();
                activity.Text = sentenceString;

                /**
                 * Call to the sentiment analytics api
                 **/

                var sentiment = await TextAnalyticsService.DetermineSentimentAsync(sentence.ToString());
                var sentimentScore = Math.Round(sentiment * 100, 1) / 10;

                /**
                 * Query to check the user issue in the QnA maker knowledge base
                 **/

                var subscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionkey"];
                var knowledgeBaseId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"];

                var responseQuery = new QnAMakerDailog.QnAMakerDialog().GetQnAMakerResponse(sentenceString, subscriptionKey, knowledgeBaseId);

                var responseAnswers = responseQuery.answers.FirstOrDefault();

                /**
                 * If the solution to the issue is found in the Kb then send the result to the user
                 **/

                if (responseAnswers != null && responseAnswers.score >= double.Parse(ConfigurationManager.AppSettings["QnAScore"]))
                {
                    await context.PostAsync(responseAnswers.answer);
                }

                /**
                 * If no solution is found then the user response from TicketModel is sent to the Dtabase APRDB and stored in dbo.BotDetails Table
                 **/

                else
                {
                    await context.PostAsync("Could not find a solution to you problem . I have raised a ticket for it, revert to you as soon as we get a solution for it");
                    try
                    {
                        string connectionEstablish = ConfigurationManager.ConnectionStrings["APRMConnection"].ConnectionString;

                        /**
                         * Establish the connection with the SQL database
                         **/

                        SqlConnection connection = new SqlConnection(connectionEstablish);

                        /**
                         * Connection to the DB being opened
                         **/

                        connection.Open();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            Console.WriteLine("Connection Success");

                            /**
                             * SQL command to insert data into the table dbo.BotDetails
                             **/

                            SqlCommand insertCommand = new SqlCommand(@"INSERT INTO dbo.BotDetails (TokenDescription, ServerDetails, MiddlewareDetails, DatabaseDetails, TokenDetails,
                                       SentimentScore, UserName, EmailId, ContactNo) VALUES (@TokenDescription, @ServerDetails, @MiddlewareDetails, @DatabaseDetails, @TokenDetails, @SentimentScore,
                                       @UserName, @EmailId, @ContactNo)", connection);

                            /**
                             * Commands to insert the user response to the database 
                             **/

                            insertCommand.Parameters.Add(new SqlParameter("TokenDescription", sentence.Desc));
                            insertCommand.Parameters.Add(new SqlParameter("ServerDetails", sentence.ServerName));
                            insertCommand.Parameters.Add(new SqlParameter("MiddlewareDetails", sentence.MiddlewareName));
                            insertCommand.Parameters.Add(new SqlParameter("DatabaseDetails", sentence.DatabaseName));
                            insertCommand.Parameters.Add(new SqlParameter("TokenDetails", System.DateTimeOffset.Now));
                            insertCommand.Parameters.Add(new SqlParameter("SentimentScore", sentimentScore));
                            insertCommand.Parameters.Add(new SqlParameter("UserName", sentence.Name));
                            insertCommand.Parameters.Add(new SqlParameter("EmailId", sentence.Contact));
                            insertCommand.Parameters.Add(new SqlParameter("ContactNo", sentence.PhoneContact));

                            var DBresult = insertCommand.ExecuteNonQuery();
                            if (DBresult > 0)
                            {
                                connection.Close();

                                string ReconnectionEstablish = ConfigurationManager.ConnectionStrings["APRMConnection"].ConnectionString;

                                SqlConnection conn = new SqlConnection(ReconnectionEstablish);

                                conn.Open();

                                var selectTicketId = "Select Id from dbo.BotDetails WHERE UserName = @UserName";

                                SqlCommand selectCommand = new SqlCommand(selectTicketId, conn);

                                selectCommand.Parameters.AddWithValue("@UserName", sentence.Name);

                                using (SqlDataReader queryReader = selectCommand.ExecuteReader())
                                {

                                    while (queryReader.Read())
                                    {
                                        String retrieveId = queryReader.GetInt32(0).ToString();
                                        //await context.PostAsync("Your ticket has been raised successfully, " + retrieveId + " your token id for the raised ticket");
                                    }

                                }

                            }

                            else
                            {
                                await context.PostAsync("Some problem occured, Please try again after sometime");
                            }

                            /**
                             * Close the existing connection to the DB
                             **/

                            connection.Close();
                        }
                        else
                        {
                            /**
                             * Checks wether the connection is established or not 
                             **/
                            Console.WriteLine("Not connected");
                        }

                        /**
                         * Snow connection code
                         **/
                        string DetailDescription = sentence.Desc + " the services are running on server " + sentence.ServerName + ", using " + sentence.DatabaseName + " database and the" + sentence.MiddlewareName + " service";
                        String incidentNo = string.Empty;
                        incidentNo = SnowLogger.CreateIncidentServiceNow(sentence.Desc, sentence.Contact, DetailDescription, sentence.CategoryName);
                        Console.WriteLine(incidentNo);
                        await context.PostAsync("Your ticket has been raised successfully, " + incidentNo + " your token id for the raised ticket");
                        await context.PostAsync("Please keep the note of above token number. as it would be used for future references");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    context.Done(this);
                }

                /**
                 * Method to check the user sentiment and report it to the user
                 */

                await context.PostAsync($"You rated our service as: {Math.Round(sentiment * 10, 1)}/10");

                if (sentiment <= 0.5)
                {
                    PromptDialog.Confirm(context, ResumeAfterFeedbackClarification, "I see it wasn't perfect, can we contact you about this?");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}