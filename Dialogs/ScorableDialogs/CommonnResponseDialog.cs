using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ProactiveBot.Dialogs.ScorableDialogs
{
    internal class CommonnResponseDialog : IDialog<object>
    {
        private string messageToSend;
        private Activity activity;

        public CommonnResponseDialog(string messageToSend)
        {
            this.messageToSend = messageToSend;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrEmpty(messageToSend))
            {
                await context.PostAsync(activity);
            }
            else
            {
                await context.PostAsync(messageToSend);
            }
            context.Done<object>(null);
        }
    }
}