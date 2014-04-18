using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("groupon")]
    public class GrouponEntity:BaseEntity
    {
        public string title { get; set; }
        public string goods_name { get; set; }
        public string desc { get; set; }
        public string attention { get; set; }
        public string pic_url { get; set; }
        public string keywords { get; set; }
        public int tel_no { get;set; }
        public int mobile { get; set; }
        public int begin_time { get; set; }
        public int end_time { get; set; }
        public int min_num { get; set; }
        public decimal original_price { get; set; }
        public decimal groupon_price { get; set; }
    }
}
