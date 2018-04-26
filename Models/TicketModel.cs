using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using ProactiveBot.SentimentAnalysis;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    public enum ContactTypeOptions
    {
        Email = 1,
        Phone,
        /*SelfService,
        WalkIn,*/
    }

    [Serializable]

    public class TicketModel : LuisDialog<object>
    {
        //Prompts for the form flow question generation

        [Prompt(new string[] { "What is your name ? " })]
        public string Name { get; set; }

        /*[Prompt(new string[] { "Tell me, How can I assist you?" })]
        public string Assist { get; set; }*/

        [Prompt(new string[] { "Give a brief description of your problem" })]
        public string Desc { get; set; }

        /*[Prompt(new string[] { "To set the priority for your ticket, tell me about how many people are affected with the problem" })]
        public string Priority { get; set; }*/

        [Prompt(new string[] { "Enter your email address ? " })]
        public string Contact { get; set; }

        [Prompt(new string[] { "Enter your contact number"})]
        public string PhoneContact { get; set; }
          
        public string ServerName {get; set;}

        [Prompt(new string[] { "What middleware services are being used by you ? "})]
        public string MiddlewareName { get; set; }

        [Prompt(new string[] { "Which database are used by you ? "})]
        public string DatabaseName { get; set; }

        [Prompt(new string[] { "Please select a category (Inquiry, Software, Hadware, Network, Database)"})]
        public string CategoryName { get; set; }

        [Prompt(new string[] { "How do you want us to contact you {||}" })]
        public ContactTypeOptions ContactType;

        public static IForm<TicketModel> BuildForm()
        {
            //Form flow builder being called

            OnCompletionAsyncDelegate<TicketModel> ConnectionRequest = async (context, state) =>
            {
                string sentenceString = state.Desc + "-" + state.DatabaseName + "-" + state.ServerName + "-" + state.MiddlewareName;

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

                    var sentiment = await TextAnalyticsService.DetermineSentimentAsync(sentenceString.ToString());
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

                                insertCommand.Parameters.Add(new SqlParameter("TokenDescription", state.Desc));
                                insertCommand.Parameters.Add(new SqlParameter("ServerDetails", state.ServerName));
                                insertCommand.Parameters.Add(new SqlParameter("MiddlewareDetails", state.MiddlewareName));
                                insertCommand.Parameters.Add(new SqlParameter("DatabaseDetails", state.DatabaseName));
                                insertCommand.Parameters.Add(new SqlParameter("TokenDetails", System.DateTimeOffset.Now));
                                insertCommand.Parameters.Add(new SqlParameter("SentimentScore", sentimentScore));
                                insertCommand.Parameters.Add(new SqlParameter("UserName", state.Name));
                                insertCommand.Parameters.Add(new SqlParameter("EmailId", state.Contact));
                                insertCommand.Parameters.Add(new SqlParameter("ContactNo", state.PhoneContact));

                                var DBresult = insertCommand.ExecuteNonQuery();
                                if (DBresult > 0)
                                {
                                    connection.Close();

                                    string ReconnectionEstablish = ConfigurationManager.ConnectionStrings["APRMConnection"].ConnectionString;

                                    SqlConnection conn = new SqlConnection(ReconnectionEstablish);

                                    conn.Open();

                                    var selectTicketId = "Select Id from dbo.BotDetails WHERE UserName = @UserName";

                                    SqlCommand selectCommand = new SqlCommand(selectTicketId, conn);

                                    selectCommand.Parameters.AddWithValue("@UserName", state.Name);

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
                            string DetailDescription = state.Desc + " the services are running on server " + state.ServerName + ", using " + state.DatabaseName + " database and the" + state.MiddlewareName + " service";
                            String incidentNo = string.Empty;
                            string contactOption;

                            /*
                             * If else condition to catch the contact option
                             */
                            if (state.ContactType == ContactTypeOptions.Email)
                            {
                                contactOption = "Email";
                            }
                             
                            else
                            {
                                contactOption = "Phone";
                            }

                            incidentNo = SnowLogger.CreateIncidentServiceNow(state.Desc, contactOption, DetailDescription, state.CategoryName);
                            Console.WriteLine(incidentNo);
                            await context.PostAsync("Your ticket has been raised successfully, " + incidentNo + " your token id for the raised ticket");
                            await context.PostAsync("Please keep the note of above token number. as it would be used for future references");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    /**
                     * Method to check the user sentiment and report it to the user
                     */

                    /*await context.PostAsync($"You rated our service as: {Math.Round(sentiment * 10, 1)}/10");

                    if (sentiment <= 0.5)
                    {
                        PromptDialog.Confirm(context, ResumeAfterFeedbackClarification, "I see it wasn't perfect, can we contact you about this?");
                    }*/
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            return new FormBuilder<TicketModel>()
                .Field(nameof(Name), validate: ValidateNameInfo)
                .Field(nameof(Desc))
                .Field(nameof(ServerName)/*validate: ValidateServerInfo*/)
                .Field(nameof(MiddlewareName), validate: ValidateMiddlewareInfo)
                .Field(nameof(DatabaseName), validate: ValidateDatabaseInfo)
                .Field(nameof(CategoryName))
                //.Field(nameof(Priority))
                .Field(nameof(ContactType))
                .Field(nameof(Contact),(s)=> s.ContactType == ContactTypeOptions.Email, validate: ValidateContactInformation)
                .Field(nameof(PhoneContact), (s) => s.ContactType == ContactTypeOptions.Phone, validate: ValidatePhoneContact)
                .AddRemainingFields()
                .Message("According to the responses entered by you I have generated a statement for you that showscase you problem : " +
                 "{Desc} running on server {ServerName}, using {DatabaseName} database and the {MiddlewareName} services used by you.")
                //"Please enter Yes if this successfully describe your problem.")
                .OnCompletion(ConnectionRequest)
                .Build();
        }

        private static Task<ValidateResult> ValidatePhoneContact(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string PhoneInfo = string.Empty;
            if (GetContactInfo((string)value, out PhoneInfo))
            {
                result.IsValid = true;
                result.Value = PhoneInfo;
                
            }

            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid contact number";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateDatabaseInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string DatabaseInfo = string.Empty;
            if(GetDatabaseInfo((string)value, out DatabaseInfo))
            {
                result.IsValid = true;
                result.Value = DatabaseInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid database information";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateMiddlewareInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string MiddlewareInfo = string.Empty;
            if (GetMiddlewareInfo((string)value, out MiddlewareInfo))
            {
                result.IsValid = true;
                result.Value = MiddlewareInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid Server information";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateServerInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string ServerInfo = string.Empty;
            if(GetServerInfo((string)value, out ServerInfo))
            {
                result.IsValid = true;
                result.Value = ServerInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid Server name";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateDescInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string descInfo = string.Empty;
            if(GetDescription((string)value, out descInfo))
            {
                result.IsValid = true;
                result.Value = descInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid response";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateContactInformation(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string contactInfo = string.Empty;
            if(GetEmailAddress((string)value, out contactInfo))
            {
                result.IsValid = true;
                result.Value = contactInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter a valid email address";
               
            }

            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateNameInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string usernameInfo = string.Empty;
            if(GetUsername((string)value, out usernameInfo))
            {
                result.IsValid = true;
                result.Value = usernameInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter name a valid name";
            }
            return Task.FromResult(result);
        }

        private static bool DescriptionEnabled(TicketModel state) =>
             !string.IsNullOrWhiteSpace(state.ServerName) && !string.IsNullOrWhiteSpace(state.Name) &&  !string.IsNullOrWhiteSpace(state.Desc) && 
             !string.IsNullOrWhiteSpace(state.MiddlewareName) && !string.IsNullOrWhiteSpace(state.DatabaseName);

        private static bool GetEmailAddress(string response, out string contactInfo)
        {
            contactInfo = string.Empty;
            var match = Regex.Match(response, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            if (match.Success)
            {
                contactInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetUsername(string value, out string usernameInfo)
        {
            usernameInfo = string.Empty;
            
            //Matches wether the if the name should start with a letter, should contain smaller case letter and should be 3 to 11 character long

            var match = Regex.Match(value, "[a-zA-z][a-z]{3,11}");
            if (match.Success)
            {
                usernameInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetDescription(string value, out string descInfo)
        {
            descInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-z]*$");
            if(match.Success)
            {
                descInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetServerInfo(string value, out string serverInfo)
        {
            serverInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-Z]");
            if (match.Success)
            {
                serverInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetMiddlewareInfo(string value, out string middlewareInfo)
        {
            middlewareInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-z$_]+");
            if (match.Success)
            {
                middlewareInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetDatabaseInfo(string value, out string databaseInfo)
        {
            databaseInfo = string.Empty;

            var match = Regex.Match(value, "[0-9a-zA-Z$_]+");
            if (match.Success)
            {
                databaseInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetContactInfo(string value, out string phoneInfo)
        {
            phoneInfo = string.Empty;

            var match = Regex.Match(value, @"^[0-9]{10}$");
            if(match.Success)
            {
                phoneInfo = match.Value;
                return true;
            }
            return false;
        }
    }
}