using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("weixin_menu_tree")]
    public class WeixinMenuTreeEntity:BaseEntity
    {
        public int parent_id { get; set; }
        public string menu_name { get; set; }
        public string command { get; set; }
        public int status { get; set; }
    }
}
