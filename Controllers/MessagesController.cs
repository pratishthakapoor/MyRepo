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
using ProactiveBot.Models;
using ProactiveBot.DatabaseConnection;

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

                // await Conversation.SendAsync(activity, () => new RaiseDialog());
                /*
                 * Log into the database
                 */

                //Instantiate the BotData dbContext
                BotDataEntities1 DB = new BotDataEntities1();
                //Create a new user log object
                Table NewUserLog = new Table();
                NewUserLog.UserID = activity.From.Id;
                NewUserLog.UserName = activity.From.Name;
                NewUserLog.TokenRaised = DateTime.UtcNow;
                NewUserLog.Issue_Details = activity.Text.Truncate(1000);
                NewUserLog.ServerName = activity.Text.Truncate(500);
                NewUserLog.MiddlewareService = activity.Text.Truncate(500);
                NewUserLog.DatbaseName = activity.Text.Truncate(500);
                //Add the Table object to Table
                DB.Tables.Add(NewUserLog);
                // Save the changes to the database 
                DB.SaveChanges();
                //Call the root dialog 
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
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
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
               string replyMessage = Responses.WelcomeMessage;
               return message.CreateReply(replyMessage);
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
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