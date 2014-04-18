using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("coupon_keys")]
    public class CouponKeysEntity:BaseEntity
    {
        public int packet_id { get; set; }
        public string coupon_key { get; set; }
        public int status { get; set; }
        public int consume_time { get; set; }
    }
}
