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
using System.Threading;

namespace Microsoft.Bot.Sample.ProactiveBot
{
        [Serializable]
		
		public class OptionDialog : IDialog<object>
		{
			//Create a dictionary to list all the options the bot service support
			
			private readonly IDictionary<string, string> options = new Dictionary<string, string>
			{
				{"1", "Virtual Machine Setup"},
				{"2", "Raise Ticket"},
				{"3", "Raise Issue"},
				{"4", "Check Ticket Status"},
			};
			
			// ShowPromptDialog method is created to show the prompt choices to the user.
        
                public virtual async Task ShowPromptDialog(IDialogContext context)
                {
					/*PromptDialog.Choice<string>(  
                    context,  
                    this. ChoiceRecievedAsync,  
                    this.options.Keys,  
                    "Please select anyone of the options",
				    4,	  
                     PromptStyle.PerLine,  
                     this.options.Values)
					 ;*/
					 PromptDialog.Choice(
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
				
				public virtual async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<string> argument)
                {
                     /*DialogBox response = await activity;
                     switch(response)
                     context.Call<object>(new FormDialog(response.ToString()), ChildDialogComplete);*/
					 var message = await argument;
					 var replyMessage = context.MakeMessage();
					 switch(message)
					 {
						 case "Virtual Machine Setup" :
						 /*PromptDialog.Choices(
							 context: context,
							 resume: ChoiceRecievedAsync,
							 prompt: "Question on Virtaul Machine Setup"
						 );*/
						 context.Call<object>(new FormDialog(), ChildDialogComplete);
						 break;
						 case "Raise Ticket" :
                         TicketDialog ticket_dialog = new TicketDialog();
                         context.Call<object>(ticket_dialog, ChildDialogComplete);
                         //await context.Forward(child: ticket_dialog, resume: ChildDialogComplete, item: msg, token: CancellationToken.None);
                         break;
						 case "Raise Issue" :
						 context.Call<object>(new TicketIssueDialog(), ChildDialogComplete);
						 break;
						 case "Check Ticket Status" :
						 //context.Call<object>(new TicketStatusDialog(), ChildDialogComplete);
					     break;
					 }
                } 
				
				public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)  
                {  
                   await context.PostAsync("Thank you for visting us.");
                   context.Done(this);  
                }
				
				public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
				{
					var message = await result;
					var welcomeMessage = context.MakeMessage();
					welcomeMessage.Text = "Welcome to the Service ChatApp, how can I help you ? ";  
                    await context.PostAsync(welcomeMessage); 
					await this.ShowPromptDialog(context);	
				}
				
				public async Task StartAsync(IDialogContext context)
                {
                   //var message = context.MakeMessage();
                   // await context.PostAsync(message);
                   //context.Wait(this.ShowPromptDialog);
                   context.Wait(this.MessageReceivedAsync);
                }
		}
}