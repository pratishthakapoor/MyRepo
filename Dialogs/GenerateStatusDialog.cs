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

namespace Microsoft.Bot.Sample.ProacticeBot
{
	[Serializable]
	
	public class GenrateStatusDialog : IDialog<object>
	{
		public async Task StartAsync(IDialogContext context)
		{
            PromptDialog.Text(
                context: context,
                resume: getTicketNo,
                prompt: "Please provide ticket number",
                retry: "Please try again later"
                );
		}

        private async Task getTicketNo(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;
            await context.PostAsync("We are checking in our database, we will get back to you");
            context.Done(this);
        }
    }
}