using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.ProactiveBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ProactiveBot.Dialogs.ScorableDialogs
{
    public class ScorableRootDialog : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public ScorableRootDialog(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            /*var message = item as IMessageActivity;
            var dialog = new RaiseDialog();
            var interruption = dialog.Void(stack);
            stack.Call(interruption, null);
            return Task.CompletedTask;*/

            var message = item as IMessageActivity;
            IDialog<IMessageActivity> interruption = null;
            if(message != null)
            {
                var incomingMessage = message.Text.ToLowerInvariant();
                var messageToSend = String.Empty;

                if(incomingMessage.Contains("how are you"))
                {
                    messageToSend = "I am good. How about you?";
                    var commonResponseDialog = new CommonnResponseDialog(messageToSend);
                    interruption = commonResponseDialog.Void<object, IMessageActivity>();
                }
                else if(incomingMessage.Contains("i am good") || incomingMessage.Contains(" i am fine") || 
                    incomingMessage.Contains(" i am fine") || incomingMessage.Contains("nice") || incomingMessage.Contains("good"))
                {
                    messageToSend = "Nice to know that";
                    var commonResponseDialog = new CommonnResponseDialog(messageToSend);
                    interruption = commonResponseDialog.Void<object, IMessageActivity>();
                }

                else if(incomingMessage.Contains("help"))
                {
                    messageToSend = Responses.HelpMessage;
                    var commonResponseDialog = new CommonnResponseDialog(messageToSend);
                    interruption = commonResponseDialog.Void<object, IMessageActivity>();
                }


                else
                {
                    if (incomingMessage.Contains("thank you"))
                        messageToSend = "You are welcome";

                    if (incomingMessage.Contains("goodbye"))
                        messageToSend = "See you later";

                    if (incomingMessage.Contains("ok") || incomingMessage.Contains("okay") || incomingMessage.Contains("fine") || incomingMessage.Contains("hmm"))
                        messageToSend = "Want to continue further";

                    if (incomingMessage.Contains("no") || incomingMessage.Contains("nope"))
                        messageToSend = "Ok, bye hope to see you soon";

                    if (incomingMessage.Contains("yup") || incomingMessage.Contains("yes") || incomingMessage.Contains("ya"))
                    {
                        messageToSend = "Ok, let's begin then";
                    }
                        


                    var commonResponseDialog = new CommonnResponseDialog(messageToSend);
                    interruption = commonResponseDialog.Void<object, IMessageActivity>();
                }

                this.task.Call(interruption, null);
                await task.PollAsync(token);
            }
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if(message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                var msg = message.Text.ToLowerInvariant();

                if(msg == "ok" || msg == "thank you" || msg == "goodbye" || msg == "fine" || msg == "hmm" || msg == "good" 
                    || msg == "nice" || msg == "okay" || msg == "how are you" || msg == "i am fine" || msg == "i am good"
                    || msg == " i am nice" || msg == "no" || msg == "nope" || msg == "yup" || msg == "yes" || msg == "ya")
                {
                    return message.Text;
                }
                else if(msg.Contains("help"))
                {
                    return message.Text;
                }
            }
            return null;
        }
    }
}