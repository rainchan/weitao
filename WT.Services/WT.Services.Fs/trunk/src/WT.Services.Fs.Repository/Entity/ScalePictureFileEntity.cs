using ServiceStack.DataAnnotations;

namespace WT.Services.Fs.Repository.Entity
{
    [Alias("file_pics")]
    public class ScalePictureFileEntity
    {
        [AutoIncrement]
        public long Id { get; set; }
        public string pic_guid { get; set; }
        public string file_guid { get; set; }
        /// <summary>
        /// 创建时间戳
        /// </summary>
        public long create_time
        {
            get;
            set;
        }
    }
}
