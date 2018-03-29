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
        
        public class RootDialog : IDialog<object>
        {
                
	       // ShowPromptDialog method is created to show the prompt choices to the user.
        
                public virtual async Task ShowPromptDialog(IDialogContext context, IAwaitable<IMessageActivity> activity)
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
                public virtual async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<DialogBox> activity)
                {
                     DialogBox response = await activity;
                     //context.Call<object>(new FormDialog(response.ToString()), ChildDialogComplete);
                } 
        
                // If the bot conversation is completed the ChildDialogComplete method will be executed 
        
                public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)  
                {  
                   await context.PostAsync("Thank you for visting us.");
                   context.Done(this);  
                } 
        
                public async Task StartAsync(IDialogContext context)
                {
                   //var message = context.MakeMessage();
                   // await context.PostAsync(message);
                   context.Wait(this.ShowPromptDialog);
                  //context.Wait(MessageReceivedAsync);
                 }
        
                 public enum DialogBox 
		{
                   VirtualMachineSetup ,
                   RaiseTicket ,
                   RaiseIssue ,
                   TicketStatus,
        }
   }
}