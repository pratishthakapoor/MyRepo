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
        private readonly IDialogStack stack;

        public ScorableRootDialog(IDialogStack stack)
        {
            SetField.NotNull(out this.stack, nameof(stack), stack);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return state != null && state == "scorable1-triggered" ? 1 : 0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null && state == "scorable1-triggered";
        }

        protected override Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;
            var dialog = new RaiseDialog();
            var interruption = dialog.Vo id(stack);
            stack.Call(interruption, null);
            return Task.CompletedTask;
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if(message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                var msg = message.Text.ToLowerInvariant();

                if(msg == "hello" || msg == "thank you" || msg == "goodbye")
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