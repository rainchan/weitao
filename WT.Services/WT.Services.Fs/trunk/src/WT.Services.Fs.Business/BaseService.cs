using System;

namespace WT.Services.Fs.Business
{
    public class BaseService
    {
        public BaseService()
        {
            //Hashtable storagerTable = ConfigurationManager.GetSection("FdfsStorages") as Hashtable;
            //Hashtable trackerTable = ConfigurationManager.GetSection("FdfsTrackers") as Hashtable;
            //FSManager.Initialize(trackerTable, storagerTable);
        }
        /// <summary>
        /// 获取16个字符的短guid
        /// </summary>
        /// <returns></returns>
        protected string GetShortGuid()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
