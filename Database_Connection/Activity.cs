using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Connection
{
    public class Activity
    {
        public string Id { get; set; }
        public string FromId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string ServerName { get; set; }
        public string MiddlewareName { get; set; }
        public string DatabaseName { get; set; }

    }
}
