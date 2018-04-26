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
using System.Data.SqlClient;
using Microsoft.Bot.Sample.ProactiveBot;

namespace Microsoft.Bot.Sample.ProacticeBot
{
	[Serializable]
	
	public class GenerateStatusDialog : IDialog<object>
	{
		public async Task StartAsync(IDialogContext context)
		{
            PromptDialog.Text(
                context: context,
                resume: getTicketNo,
                prompt: "Please provide ticket number",
                retry: "Please try again later"
                );
		}

        private async Task getTicketNo(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;
            //await context.PostAsync("We are checking in our database, we will get back to you");

            /**
             * Connect to the database to retrieve the ticket details 
             **/

            //string establishConnection = ConfigurationManager.ConnectionStrings["APRMConnection"].ConnectionString;

            //SqlConnection sqlConnection = new SqlConnection(establishConnection);

            /**
             * Open the connection to the SQL database 
             **/
            //sqlConnection.Open();

            try
            {
                /**
             * SQL Select query to retireve the ticket status and details
             **/
                //var SelectQuery = @"SELECT Id from dbo.BotDetails WHERE Id = @Id";

                //SqlCommand selectCommand = new SqlCommand(SelectQuery, sqlConnection);

                //selectCommand.Parameters.AddWithValue("@Id", response);

                // Call to the SQL data reader

                /*using (SqlDataReader dataReader = selectCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        String retrieveId = dataReader.GetInt32(0).ToString();

                        /**
                         * the if condition checks the wether the Ticket Id enetered by the user matches the id stored in the DB
                         **/

                        /*if (retrieveId == response)
                        {
                            Console.WriteLine("Perfect Match occured");
                            await context.PostAsync("A  ticket for this ID id sucessfully found");
                        }
                    } 
      
                    // close the data reader
                    //dataReader.Close();

                    await context.PostAsync("We have not found any details against the given ticket id. Please check the details.");

                }*/

                //close the sql connection to the database

                //sqlConnection.Close();

                /**
                 * Method call to handle the user request to check ticket status from the SNOW service
                 * Read data from the SNOW incidence
                 **/

                /**
                 * statusDetails parameter stores the value return by RetrieveIncidentServiceNow method of the Snow Logger class
                 **/

                string statusDetails = SnowLogger.RetrieveIncidentServiceNow(response);

                /**
                 * The if- else- if condition to match the state of the incident token returned by the RetrieveIncidentSerivceNow method
                 */

                if (statusDetails == "1")
                {
                    var status = "Your token is created and is under review by our team.";
                    string Notesresult = SnowLogger.RetrieveIncidentWorkNotes(response);

                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(Notesresult, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    await context.PostAsync(replyMessage);

                }
                    
                else if (statusDetails == "2")
                {
                    var status = "Your ticket is in progress.";
                    string Notesresult = SnowLogger.RetrieveIncidentWorkNotes(response);

                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(Notesresult, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    await context.PostAsync(replyMessage);

                }
                    
                else if (statusDetails == "3")
                {
                    await context.PostAsync("Your ticket is been kept on hold.");

                    
                }
                    
                else if (statusDetails == "6")
                {
                    var status = "Your ticket is resolved.";

                    /**
                     * Retrieves the details from the resolve columns of SnowLogger class if the incident token is being resolved
                     **/

                    string resolveDetails = SnowLogger.RetrieveIncidentResolveDetails(response);
                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(resolveDetails, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    //await context.PostAsync("For the ticket id " + response+ " solution fetched by our team is : " + resolveDetails);
                    await context.PostAsync(replyMessage);
                }
                   
                    
                else if (statusDetails == "7")
                {
                    var status = "Your ticket has been closed by our team";

                    /**
                     * Retrieves the close_code from the SnowLogger class if the incident token is being closed
                     **/

                    string resolveDetails = SnowLogger.RetrieveIncidentCloseDetails(response);
                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(resolveDetails, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    //await context.PostAsync("Reasons for closing the ticket: " + resolveDetails);
                    await context.PostAsync(replyMessage);
                }
                    
                else
                    await context.PostAsync("Our team cancelled your ticket");

               // Console.WriteLine(statusDetails);
            }

            /**
             * Catch any unhandle exception thrown by the try block
             **/

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            context.Done(this);
        }

        private Attachment GetReplyMessage(string notesresult, string response, string status)
        {
            /**
             * Hero card creation 
             **/
             
            var heroCard = new HeroCard
            {
                //title for the given
                Title = "Progress details for the ticket " + response,
                // subtitle for the card
                Subtitle = status,
                //Detail text
                Text = "Latest work carried out on your raised ticket includes:\n\n" + notesresult,
                //in case for other channel use
                /**
                 * Text = "Latest work carried out on your raised ticket includes:\n\n" + notesresult, ex : Text = "More words <br> New line <br> New line <b><font color = \"#11b92f\>GREEN</font></b></br></br>
                 **/
                //list of buttons
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Need further details? ", value: "https://www.t-systems.hu/about-t-systems/customer-contact/service-desk"),
                    new CardAction(ActionTypes.OpenUrl, "Contact us at", value: "https://www.t-systems.com/de/en/contacts")}
            };
            return heroCard.ToAttachment();
        }

        
    }
}