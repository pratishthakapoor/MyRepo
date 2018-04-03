using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlConnection
{
    public class Activity
    {
        public string Id { get; set; }
        public string questions { get; set; }
        public string server_details { get; set; }
        public string middleware_details { get; set; }
        public string database_details { get; set; }
        public DateTimeOffset token_details { get; set; }
    }
}
