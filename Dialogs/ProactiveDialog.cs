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
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.ProactiveBot

{
    [Serializable] 
    
    // define an enum for different type of options available
    

    public class ProactiveDialog : IDialog<object>
    {
        
        // ShowPromptDialog method is created to show the prompt choices to the user.
        
       /* public virtual async Task ShowPromptDialog(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;
            PromptDialog.Choice(
                context: context,
                resume: ChoiceRecievedAsync,
                options: (IEnumerable<DialogBox>)Enum.GetValues(typeof(DialogBox)),
                prompt: " Hi, How can I assist you : ",
                retry: "Oops, Some problem occured. Please try again.",
                promptStyle: PromptStyle.Auto
            );
        }
        */
        // When the user selects an option, ChoiceRecievedAsync method will be called.
        
        /*public virtual async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<DialogBox> activity)
        {
            DialogBox response = await activity;
            context.Call<object>(new FormDialog(response.ToString()), ChildDialogComplete);
            
        } 
        
        // If the bot conversation is completed the ChildDialogComplete method will be executed 
        
        public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)  
        {  
            await context.PostAsync("Thank you for visting us.");
            context.Done(this);  
        } */
        
        public async Task StartAsync(IDialogContext context)
        {
            //var message = context.MakeMessage();
            // await context.PostAsync(message);
            //context.Wait(this.ShowPromptDialog);
            context.Wait(MessageReceivedAsync);
        }
        
        /* public enum DialogBox {
                 VirtualMachineSetup ,
                 RaiseTicket ,
                 RaiseIssue ,
                 TicketStatus,
              }*/
 
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
                // Create a queue Message
                var queueMessage = new Message
                {
                    RelatesTo = context.Activity.ToConversationReference(),
                    Text = message.Text
                };
                

                // write the queue Message to the queue
                await AddMessageToQueueAsync(JsonConvert.SerializeObject(queueMessage));

                //await context.PostAsync($"You said {queueMessage.Text}. Your message has been added to a queue, and it will be sent back to you via a Function shortly.");
              
                context.Wait(MessageReceivedAsync);
        }

        public static async Task AddMessageToQueueAsync(string message)
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); // If you're running this bot locally, make sure you have this appSetting in your web.config

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            var queue = queueClient.GetQueueReference("bot-queue");

            // Create the queue if it doesn't already exist.
            await queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue.
            var queuemessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(queuemessage);
        }

    }
}