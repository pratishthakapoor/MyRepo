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
using System.Threading;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]

    public class TicketDialog : LuisDialog<object>
    {
        //string IssueDsecription;
        //string Username;
        //string TokenNo;
        //string RaiseToken;
        //string IssueType;

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            var cts = new CancellationTokenSource();
            await context.Forward(new OptionDialog(), GreetingDialogDone, await message, cts.Token);
        }


        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.HelpMessage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Raise Ticket")]
        public async Task RaiseTicket(IDialogContext context, LuisResult result)
        {
            try
            {
                var ticketForm = new FormDialog<TicketModel>(new TicketModel(), TicketModel.BuildForm, FormOptions.PromptInStart);
                context.Call(ticketForm, TicketFormComplete);
            }
            catch(Exception)
            {
                await context.PostAsync("Some problem occured. You can try again later after some time.");
                context.Wait(MessageReceived);
            }
        }

        private static string recipientEmail = ConfigurationManager.AppSettings["RecipientEmail"];
        private static string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
        //private string call_intent;

        //string optiondetails;

        /*public TicketDialog(string option)
        {
            optiondetails = option;
        }*/


        private async Task TicketFormComplete(IDialogContext context, IAwaitable<TicketModel> result)
        {
            try
            {
                var token = await result;
                string message = GenerateEmailMessage(token);
                var success = await EmailSender.SendEmail(message, recipientEmail, senderEmail, $"Email from {token.Name}");
                if (!success)
                    await context.PostAsync("I was not able to send your message. Something went wrong.");
                else
                {
                    await context.PostAsync("Thanks for the responses.");
                    await context.PostAsync("What else would you like to do?");
                }
            }
            catch(FormCanceledException)
            {
                await context.PostAsync("Don't want to raise a issue? ");
            }
            catch(Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
            }
            finally
            {
                context.Wait(MessageReceived);
            }
        }

        private string GenerateEmailMessage(TicketModel token)
        {
            throw new NotImplementedException();
        }

        private async Task GreetingDialogDone(IDialogContext context, IAwaitable<object> result)
        {
            var success = await result;
            /*if (!success)
            {
                await context.PostAsync("I'm sorry. I didn't understand you.");
            }*/

            context.Wait(MessageReceived);
        }

        /*public async Task StartAsync(IDialogContext context)
          {
            PromptDialog.Text(
            context : context,
            resume : getUsername,
            prompt : "What is your name "
            );
           }*/

        /*public async Task getUsername(IDialogContext context, IAwaitable<string> name)
		{
			PromptDialog.Text(
				context: context,
				resume: MessageRecievedAsync,
				prompt: "Tell me what problem you are facing ? "
			);	
		}*/

        /*public virtual async Task MessageRecievedAsync(IDialogContext context, IAwaitable<string> result)
		{
			var response = await result;
			IssueType = response;
			PromptDialog.Text(
				context: context,
				resume: getMoreInfo,
				prompt : "Give a brief description about the problem"
			);
			
		}*/

        /*public virtual async Task getMoreInfo(IDialogContext context, IAwaitable<string> Info)
		{
			var response = await Info;
			IssueDsecription = response;
			PromptDialog.Choice<string>(
				context,
				this.getSuggestionAsync,
				this.options.Values,
				"What you want me to do on your behalf ?",
				"Oops, Some problem occured. Please try again.",
				2,
				PromptStyle.Auto,
				this.options.Values
			);
		}*/

        /*public virtual async Task getSuggestionAsync(IDialogContext context, IAwaitable<string> argument)
		{
			var message = await argument;
			var replymessage = context.MakeMessage();
			switch(message)
			{
				case "Raise a ticket":
				context.Call<object>(new RaiseDialog(), ChildDialogComplete);
				break; 
				case "Contact a human":
				//context.Call<object>(new GenrateStatusDialog(),HumanDialogComplete);
				break;
				
			}
		}*/

        /*public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)
		{
			await context.PostAsync("Thank you for visting us.");
            context.Done(this);  
		}
		
		public virtual async Task HumanDialogComplete(IDialogContext context, IAwaitable<object> response)
		{
			await context.PostAsync("Thanks for contacting us. Hope your problem got solved");
			context.Done(this);
		}*/

        /*private readonly IDictionary<string, string> options = new Dictionary<string, string>
        {
            {"1", "Raise a ticket"},
            {"2", "Would you like to make some changes ? "},
        };*/
    }
}  