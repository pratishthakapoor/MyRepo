using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Sample.ProactiveBot;

namespace ProactiveBot.Dialogs
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]

    public class RaiseTicketDialog : LuisDialog<object>
    { 
        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. Didn't understand that");
            context.Wait(MessageReceived);
        }

   
        [LuisIntent("Raise Ticket")]
        public async Task AboutMe(IDialogContext context, LuisResult result)
        {
            
            try
            {
                var ticketForm = new FormDialog<TicketModel>(new TicketModel(), TicketModel.BuildForm, FormOptions.PromptInStart);
                context.Call(ticketForm, TicketFormComplete);
            }
            catch (Exception)
            {
                await context.PostAsync("Some problem occured. You can try again later after some time.");
                context.Wait(MessageReceived);
            }
        }


        private static string recipientEmail = ConfigurationManager.AppSettings["RecipientEmail"];
        private static string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];

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
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to raise a issue? ");
            }
            catch (Exception)
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

    }
}