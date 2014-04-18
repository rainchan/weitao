using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    [Alias("members")]
    public class MembersEntity:BaseEntity
    {
        public int card_id { get; set; }
        public int member_level { get; set; }
        public string user_name { get; set; }
        public int mobile { get; set; }
        public string email { get; set; }
        public string wexin { get; set; }
        public int score { get; set; }
        public int status { get; set; }
    }
}
