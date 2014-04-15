using System;

namespace WT.Services.AccessToken.Business.Common
{
    public class CommonHelper
    {
        public static long GetVersion
        {
            get
            {
                return (DateTime.Now - DateTime.Parse("1970-01-01 00:00:00")).Ticks;
            }
        }
    }
}
