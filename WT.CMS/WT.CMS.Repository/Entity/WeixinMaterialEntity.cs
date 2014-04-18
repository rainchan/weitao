using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("weixin_material")]
    public class WeixinMaterialEntity:BaseEntity
    {
        public string title { get; set; }
        public string pic_url { get; set; }
        public string summary { get; set; }
        public string content { get; set; }
        public string intent_href { get; set; }
        public string local_href { get; set; }
    }
}
