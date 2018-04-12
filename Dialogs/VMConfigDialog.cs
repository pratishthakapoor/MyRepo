using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [Serializable]
    internal class VMConfigDialog : IDialog<object>
    {
        /**
         * Dictionary to store the various VM options
         **/

        public IDictionary<string, string> VMOptions = new Dictionary<string, string>
        {
            {"1", "New VM creation" },
            {"2", "Adding storage to an existing VM " }
        };

        /**
         * StartAsync is the method implemented to mark the start of a IDialog
         **/

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

        /**
         * Resume method being carried out after the PromptDialog creation for different VM options
         **/

        private async Task ResumeAfterChoiceClarification(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;

            /**
             * Switch case to handle different choice made by the user from the VMOptions Dialog
             **/

            switch (message)
            {
                // Case for the VM creation and setup

                case "New VM creation":
                    await context.PostAsync($"I would require some details to create a VM for you");
                    context.Call(new FormDialog(), ChildDialogComplte);
                break;

                // Case for adding extra storage to the existing VM

                case "Adding storage to an existing VM":
                    context.Call(new StorageAdditionDialog(), ChildDialogComplte);
                break;
            }
        }

        /**
         * Child dialog to handle the activity when the VMConfigDialog is closed
         **/

        private async Task ChildDialogComplte(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(this);
        }
    }
}