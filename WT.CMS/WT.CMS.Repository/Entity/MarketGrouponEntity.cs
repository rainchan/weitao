using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("market_groupon")]
    public class MarketGrouponEntity:BaseEntity
    {
        public int market_id{get; set; }
        public int groupon_id { get; set; }
    }
}
