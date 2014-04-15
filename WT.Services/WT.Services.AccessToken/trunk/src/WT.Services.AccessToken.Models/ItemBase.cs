using System;

namespace WT.Services.AccessToken.Models
{
    public abstract class ItemBase
    {

        public ItemBase()
        {
            Version = this.GetVersion();
            CreateDateTime = DateTime.Now;
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public Int64 Version
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime
        {
            get;
            private set;
        }

        private long GetVersion()
        {            
            return (DateTime.Now - DateTime.Parse("1970-01-01 00:00:00")).Ticks;            
        }
    }
}
