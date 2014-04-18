using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("markets")]
    public class MarketsEntity:BaseEntity
    {
        public string market_name { get; set; }
        public string address { get; set; }
        public int tel_no { get; set; }
        public int mobile { get; set; }
        public string keywords { get; set; }
        public string contact { get; set; }
    }
}
