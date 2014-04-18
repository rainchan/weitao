using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT.CMS.Repository.Entity
{
    public class PacketsEntity
    {
        public string packet_name { get; set; }
        public int market_id { get; set; }
        public int begin_time { get; set; }
        public int end_time { get; set; }
        public int max_expire_day { get; set; }
        public int max_issue { get; set; }
        public int mininum { get; set; }
    }
}
