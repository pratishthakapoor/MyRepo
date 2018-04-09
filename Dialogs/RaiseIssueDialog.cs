using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [Serializable]
    public class RaiseIssueDialog : IDialog<object>
    {
        private const string Options = "Raise Ticket";
        string UserName;

        public enum Choice
        {
            RaiseTicket
        }

        // Dictionary being used to show all the bot features to the user
        private readonly IDictionary<string, string> Issueoptions = new Dictionary<string, string>
            {
                //{"1", "Problem in raising a ticket"},
                {"2", "Mailing Issue"},
                {"3", "Not able to contatct support team"},
                {"4", "Printer Issue"},
                {"5", "Problem in Login"}
            };

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context,
                     this.ChoiceRecievedAsync,
                     this.Issueoptions.Values,
                     "Please select anyone of the options ",
                     "Oops, Some problem occured. Please try again.",
                     5,
                     PromptStyle.Auto,
                     this.Issueoptions.Values
            );
        }

        public async Task ChoiceRecievedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var response = await argument;
            var replyMessage = response;
            switch(response)
            {
                /*case "Problem in raising a ticket":
                    await context.PostAsync($"Please wait for a while we are moving you to the raise ticket option");
                    //context.Call(new RaiseDialog(),ChildDialogComplete);
                    var options = new Choice[] { Choice.RaiseTicket };
                    var descriptions = new string[] { "Raise Ticket" };
                    PromptDialog.Choice(
                        /*context: context,
                        options: (IEnumerable<Choice>)Enum.GetValues(typeof(Choice)),
                        resume: NewDialogCompleteAsync,
                        prompt: "Want to raise a ticket",
                        retry: "Sorry didn't understand that. Please try again"
                        context, NewDialogCompleteAsync, options, "Want to raise a ticket ? ", descriptions : descriptions);
                    break;*/
                case "Mailing Issue":
                    await context.PostAsync($"Please contact us at info@t-systems.com");
                    context.Done(this);
                    break;
                case "Printer Issue":
                    await context.PostAsync($"Please coonect to our service help desk at +91 2038005000 ");
                    context.Done(this);
                    break;
                case "Not able to contatct support team":
                    await context.PostAsync($"You can contact us at FMB.FMB-FMB-TSINPresales@t-systems.com");
                    context.Done(this);
                    break;
                case "Problem in Login":
                      PromptDialog.Text(
                        context: context,
                        resume: getUserName,
                        prompt: "Please enter your email address here",
                        retry: "Sorry didn't understand that. Please try again"
                     );
                    context.Done(this);
                   break;
            }
        }

        private async Task NewDialogCompleteAsync(IDialogContext context, IAwaitable<Choice> result)
        {
            context.Call(new RaiseDialog(), ChildDialogComplete);
            var response = await result;
            Activity myActivity = (Activity)context.Activity;
            myActivity.Text = response.ToString();
            await MessageReceived(context, Awaitable.FromItem(myActivity));
            
        }

        public async Task getUserName(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;
            UserName = response;
            await context.PostAsync($"We will be contacting you back in sometime");
        }

        private Task MessageReceived(IDialogContext context, IAwaitable<Activity> awaitable)
        {
            throw new NotImplementedException();
        }

        private async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(this);
        }
    }
}