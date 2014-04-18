using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("groupon_type")]
    public class GrouponTypeEntity:BaseEntity
    {
        public string type_name { get; set; }
        public int enabled { get; set; }
        public int menu_id { get; set; }
    }
}
