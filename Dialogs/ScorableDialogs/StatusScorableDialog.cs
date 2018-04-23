using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.ProacticeBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ProactiveBot.Dialogs.ScorableDialogs
{
    public class StatusScorableDialog : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public StatusScorableDialog(IDialogTask task)
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
            var message = item as IMessageActivity;

            if (message != null)
            {
                var StatusDialog = new GenerateStatusDialog();

                var interruption = StatusDialog.Void<object, IMessageActivity>();

                this.task.Call(interruption, null);

                await this.task.PollAsync(token);
            }
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if(message != null)
            {
                if(message != null && string.IsNullOrWhiteSpace(message.Text))
                {
                    if(message.Text.Equals("Check previous raised Ticket Status", StringComparison.InvariantCultureIgnoreCase)
                        && message.Text.Equals("Tell me about my previous raised ticket", StringComparison.InvariantCultureIgnoreCase) &&
                        message.Text.Equals("Show me my prevoius raised ticket", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return message.Text;
                    }
                }
            }
            return null;
        }
    }
}