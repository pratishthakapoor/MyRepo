using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProactiveBot.Dialogs;
using System.Data.Entity.Infrastructure;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using AdaptiveCards;
using System.Threading;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        /// [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity.GetActivityType() == ActivityTypes.Message)
            {

                //await Conversation.SendAsync(activity, () => new RaiseDialog());
                /*
                 * Log into the database
                 */

                //Instantiate the BotData dbContext
                /*BotDataEntities1 DB = new BotDataEntities1();
                //Create a new user log object
                Table NewUserLog = new Table
                {
                    UserID = activity.From.Id,
                    UserName = activity.From.Name,
                    TokenRaised = DateTime.UtcNow,
                    Issue_Details = activity.Text.Truncate(1000),
                    ServerName = activity.Text.Truncate(50),
                    MiddlewareService = activity.Text.Truncate(50),
                    DatbaseName = activity.Text.Truncate(50)
                };
                try
                {
                    //Add the Table object to Table
                    DB.Tables.Add(NewUserLog);
                    // Save the changes to the database 
                    DB.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }*/

                //Call the root dialog 

                var connectorClient = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                Activity isTyping = activity.CreateReply("Bot is typing...");
                //Thread.Sleep(5000);
                isTyping.Type = ActivityTypes.Typing;
                await connectorClient.Conversations.ReplyToActivityAsync(isTyping);

                await Conversation.SendAsync(activity, () => new RaiseDialog());



            }
            else if (activity.Type == ActivityTypes.Event)
            {
                IEventActivity triggerEvent = activity;
                var message = JsonConvert.DeserializeObject<Message>(((JObject)triggerEvent.Value).GetValue("Message").ToString());
                var messageactivity = (Activity)message.RelatesTo.GetPostToBotMessage();

                var client = new ConnectorClient(new Uri(messageactivity.ServiceUrl));
                var triggerReply = messageactivity.CreateReply();
               
                triggerReply.Text = $"{message.Text}";
                await client.Conversations.ReplyToActivityAsync(triggerReply);
            }
            else
            {
                HandleSystemMessageAsync(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                /*string replyMessage = Responses.WelcomeMessage;
                return message.CreateReply(replyMessage);*/
               IConversationUpdateActivity conversationUpdateActivity = message as IConversationUpdateActivity;
               //IMessageActivity messageActivity = message as IMessageActivity;
                if(conversationUpdateActivity != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
                    foreach(var memeber in conversationUpdateActivity.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        if (memeber.Id == conversationUpdateActivity.Recipient.Id)
                        {
                            /* var reply = ((Activity)conversationUpdateActivity).CreateReply($"Welcome, to Service Chat App");
                             await connector.Conversations.ReplyToActivityAsync(reply);*/
                            Activity reply = ((Activity)conversationUpdateActivity).CreateReply($"Welcome, to Service ChatBot");

                            AdaptiveCard adaptiveCard = new AdaptiveCard();

                            //Add an image to the card
                            adaptiveCard.Body.Add(new AdaptiveCards.Image()
                            {
                                Url = "https://ansiblergdiag813.blob.core.windows.net/chat-bot-images/images.png",
                                Size = ImageSize.Large,
                                Style = ImageStyle.Person,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Separation = SeparationStyle.Strong,
                                Type = "Image"
                            });

                            // Add text body to the adative card
                            adaptiveCard.Body.Add(new TextBlock()
                            {
                                Text = "I am ServiceChat Bot, designed to resolve your problems. ",
                                Size = TextSize.Large,
                                Weight = TextWeight.Lighter,
                                MaxLines = 2,
                                Wrap = true,
                                Color = TextColor.Accent,
                                IsSubtle = true
                            });

                            // Add text to start the conversation
                            adaptiveCard.Body.Add(new TextBlock()
                            {
                                Text = "To start the conversation, Please enter Hi",
                                Size = TextSize.Medium,
                                Weight = TextWeight.Lighter,
                                MaxLines = 2,
                                Wrap = true,
                                Color = TextColor.Dark,
                                IsSubtle = true
                            });

                            //Create an attachment for the adaptive card
                            Attachment attachment = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = adaptiveCard
                            };

                            reply.Attachments.Add(attachment);

                            var replytoconversation = await connector.Conversations.SendToConversationAsync(reply);
                          
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                IConversationUpdateActivity conversationUpdateActivity = message as IConversationUpdateActivity;
                if (conversationUpdateActivity != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
                    foreach (var memeber in conversationUpdateActivity.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        if (memeber.Id == conversationUpdateActivity.Recipient.Id)
                        {
                            var reply = ((Activity)conversationUpdateActivity).CreateReply($"Welcome, to Service Chat App");
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing

                /*var connectorClient = new ConnectorClient(new System.Uri(message.ServiceUrl));
                Activity isTyping = message.CreateReply("Bot is typing...");
                //Thread.Sleep(5000);
                //isTyping.Type = ActivityTypes.Typing;
                await connectorClient.Conversations.ReplyToActivityAsync(isTyping);*/

            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
    }

    public class Message
    {
        public ConversationReference RelatesTo { get; set; }
        public String Text { get; set; }
    }
}