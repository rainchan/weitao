using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace WT.CMS.Repository.Entity
{
    public class BaseEntity
    {
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// 创建时间戳
        /// </summary>
        public long create_time { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public long last_changed_time { get; set; }
    }
}
