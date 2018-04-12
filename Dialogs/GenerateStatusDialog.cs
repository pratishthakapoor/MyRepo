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

namespace Microsoft.Bot.Sample.ProacticeBot
{
	[Serializable]
	
	public class GenrateStatusDialog : IDialog<object>
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

            string establishConnection = ConfigurationManager.ConnectionStrings["APRMConnection"].ConnectionString;

            SqlConnection sqlConnection = new SqlConnection(establishConnection);

            /**
             * Open the connection to the SQL database 
             **/
            sqlConnection.Open();

            try
            {
                /**
             * SQL Select query to retireve the ticket status and details
             **/
                var SelectQuery = @"SELECT Id from dbo.BotDetails WHERE Id = @Id";

                SqlCommand selectCommand = new SqlCommand(SelectQuery, sqlConnection);

                selectCommand.Parameters.AddWithValue("@Id", response);

                // Call to the SQL data reader

               using(SqlDataReader dataReader = selectCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        String retrieveId = dataReader.GetInt32(0).ToString();

                        /**
                         * the if condition checks the wether the Ticket Id enetered by the user matches the id stored in the DB
                         **/

                        if (retrieveId == response)
                        {
                            Console.WriteLine("Perfect Match occured");
                            await context.PostAsync("A  ticket for this ID id sucessfully found");
                        }

                        else
                        {
                            await context.PostAsync("We have not found any details against the given ticket id. Please check the details.");
                        }
                    }

                    // close the data reader
                    dataReader.Close();
                }

                //close the sql connection to the database

                sqlConnection.Close();
            }

            /**
             * Catch any unhandle exception thrown by the try block
             **/

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            context.Done(this);
        }
    }
}