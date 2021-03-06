﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [Serializable]
    internal class ResetPasswordDialog : IDialog<object>
    {
        String CustomerId;
        String ServerDetails;
        String EmailId;

        /*public IDictionary<string, string> VMOptions = new Dictionary<string, string>
        {
            {"1", "New VM creation" },
            {"2", "Adding storage to an existing VM " }
        };*/

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context: context,
                resume : getCustomerId,
                prompt : "Please provide your Customer ID",
                retry : "I didn't understand that, Please try again"
                );
        }

        private async Task getCustomerId(IDialogContext context, IAwaitable<string> CustId)
        {
            var response = await CustId;
            CustomerId = response;
            PromptDialog.Text(
                context,
                resume : getServerDetails,
                prompt: "Please provide your server name/IP address of the server",
                retry : " I didn't understand that, Please try again"
                );
        }

        private async Task getServerDetails(IDialogContext context, IAwaitable<string> Details)
        {
            var response = await Details;
            ServerDetails = response;
            PromptDialog.Text(
                context,
                resume: getUserEmailId,
                prompt: "Please provide your Email Id",
                retry : "I didn't understand that, Please try again"
                );
        }
        
        private async Task getUserEmailId(IDialogContext context, IAwaitable<string> Email)
        {
            var response = await Email;
            EmailId = response;
            await context.PostAsync("Password has been reset successfully, you will recieve a email please confirm after login.");
            context.Done(this);
        }
    }
}