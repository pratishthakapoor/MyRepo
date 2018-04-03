using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlConnection
{
    public class ConversationDataContext : DbContext
    {
        public ConversationDataContext() : base("APRMConnection")
        {

        }
        public DbSet<Activity> Activities { get; set; }
    }
}
