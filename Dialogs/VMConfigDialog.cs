using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [Serializable]
    internal class VMConfigDialog : IDialog<object>
    {
        public IDictionary<string, string> VMOptions = new Dictionary<string, string>
        {
            {"1", "New VM creation" },
            {"2", "Adding storage to an existing VM " }
        };

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.ResumeAfterChoiceClarification,
                this.VMOptions.Values,
                "Please select anyone of the options ",
                "Oops, Some problem occured. Please try again.",
                 2,
                 PromptStyle.Auto,
                 this.VMOptions.Values
                );  
        }

        private async Task ResumeAfterChoiceClarification(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            switch(message)
            {
                case "New VM creation":
                    context.Call(new FormDialog(), ChildDialogComplte);
                break;
                case "Adding storage to an existing VM":
                    context.Call(new StorageAdditionDialog(), ChildDialogComplte);
                break;
            }
        }

        private async Task ChildDialogComplte(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(this);
        }
    }
}