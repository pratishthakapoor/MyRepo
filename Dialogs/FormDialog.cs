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
using System.Collections.Generic;
using ProactiveBot.SentimentAnalysis;

namespace Microsoft.Bot.Sample.ProactiveBot
{
	[Serializable]
	
	public class FormDialog: IDialog<object>
	{
           
           string VirtualName;
           string VirtualOSName;
           string VirtualCloudProvider;
	         public FormDialog()
            {
              //plandetails = plan;
            }
            
            //Async method is being called first when a new Dialog is created
            
           public async Task StartAsync(IDialogContext context)
           {
                 
             //await context.PostAsync(" Do you want any Help on "+ plandetails + " so i can help");
             //context.Wait(MessageRecievedAsync);
             PromptDialog.Text(
                context : context,
                resume: getVmName,
                prompt : "Please Enter the name of the Virtual Machine that you want to create",
                retry: "Sorry didn't understand that. Please try again"
              );
            }
           
            /*public async Task MessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
            {
              
            }*/
            
            // Method to get the name of the VM as response
            
            public virtual async Task getVmName(IDialogContext context, IAwaitable<string> VMname)
            {
              var response = await VMname;
              VirtualName = response;
              PromptDialog.Choice(
                context : context,
                options : (IEnumerable<ChoiceOS>)Enum.GetValues(typeof(ChoiceOS)),
                resume : getChooseOS,
                prompt: "Which OS do you choose ? ",
                retry : "Oops, some error occured.Please select again",
                promptStyle: PromptStyle.Auto
              );
            }
            
            // Method that allows the user to choose the OS platform for their VM
            
            public virtual async Task getChooseOS(IDialogContext context, IAwaitable<ChoiceOS> NameOS)
            {
              var response = await NameOS;
              VirtualOSName = response.ToString();
              PromptDialog.Choice(
                context : context,
                options : (IEnumerable<CloudServiceProvider>)Enum.GetValues(typeof(CloudServiceProvider)),
                resume : getCloudProvider,
                prompt : "Select a CLoud service provider to host you VM ",
                retry : "Oops, please select again",
                promptStyle : PromptStyle.Auto
              );    
            }
          
          // Method that gets the Cloud service provider response form user
          
          public virtual async Task getCloudProvider(IDialogContext context, IAwaitable<CloudServiceProvider> cloudProvider)
            {
              var response = await cloudProvider;
              VirtualCloudProvider = response.ToString();
            await context.PostAsync(String.Format("Your request for virtual machine creation is under progress.We will update you via Email."));
                  //"Your Virtual Mchine {0} is ready to run on {1} and supports {2} platform.", VirtualName, VirtualCloudProvider, VirtualOSName ));
              var sentence = response;
              var sentiment = await TextAnalyticsService.DetermineSentimentAsync(sentence.ToString());
              await context.PostAsync($"You rated our service as: {Math.Round(sentiment * 10, 1)}/10");

              if (sentiment < 0.5)
              { 
                  PromptDialog.Confirm(context, ResumeAfterFeedbackClarification, "I see it wasn't perfect, can we contact you about this?");
              }
              context.Done(this);
            }

          private async Task ResumeAfterFeedbackClarification(IDialogContext context, IAwaitable<bool> result)
          {
             var confirmation = await result;
             await context.PostAsync(confirmation ? "We'll call you!" : "We won't contact you.");
             context.Done(this);
          }

        // Enumeration being created for the different OS platform

        public enum ChoiceOS
            {
              Windows,
              Linux,
              Unix,
              RedHat,
              Mac,
            }
            
            // enum for the various Cloud Service Provider
            
            public enum CloudServiceProvider
            {
              AmazonWebService,
              MicrosoftAzure,
              IBMCloud,
              GoogleCloudPlatform,
              Racksapce,
            }
	   }
}