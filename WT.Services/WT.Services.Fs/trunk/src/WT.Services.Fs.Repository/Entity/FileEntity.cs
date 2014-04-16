using WT.Components.Database;
using ServiceStack.DataAnnotations;

namespace WT.Services.Fs.Repository.Entity
{
    [Alias("autopia_files")]
    public class FileEntity : BaseEntity
    {
        public string file_guid { get; set; }
        public string file_full_name { get; set; }
        public string file_path { get; set; }
        public string file_md5 { get; set; }
        public long file_size { get; set; }
    }
}
