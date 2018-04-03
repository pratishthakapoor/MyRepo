using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProactiveBot.Database
{
    public class EntityFrameworkActivityLogger : IActivityLogger
    {
        public Task LogAsync(IActivity activity)
        {
            IMessageActivity msg = activity.AsMessageActivity();
            using (SqlConnection.ConversationDataContext dataContext = new SqlConnection.ConversationDataContext())
            {
                var newActivity = AutoMapper.Mapper.Map<IMessageActivity, SqlConnection.Activity>(msg);
                if (string.IsNullOrEmpty(newActivity.Id))
                    newActivity.Id = Guid.NewGuid().ToString();

                dataContext.Activities.Add(newActivity);
                dataContext.SaveChanges();
            }
            return null;
        }
    }
}