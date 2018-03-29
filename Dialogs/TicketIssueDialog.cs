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

namespace Microsoft.Bot.Sample.ProactiveBot
{
	[Serializable]
	
	public class TicketIssueDialog : IDialog<object>
	{
		public async Task StartAsync (IDialogContext context)
		{
			
		}
	}
}