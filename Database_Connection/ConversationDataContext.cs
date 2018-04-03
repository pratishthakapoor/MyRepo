using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Connection
{
    public class ConversationDataContext : DbContext
    {
        protected ConversationDataContext() : base("APRMConnection")
        {
        }

        public DbSet<Activity> Activities { get; set; }

    }
}
