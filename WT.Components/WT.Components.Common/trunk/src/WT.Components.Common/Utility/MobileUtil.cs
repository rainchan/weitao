namespace WT.Components.Common.Utility
{
    public class MobileUtil
    {
        /**
         * 判断传入的参数号码为哪家运营商
         * @param mobile
         * @return 运营商名称 1 移动， 2联通， 3电信， -1不明
         */
        public static int validateMobile(string mobile)
        {
            int retType = 0;
            if (mobile == null || mobile.Length != 11 || !StringUtil.isDigitl(mobile))
            {
                return -1;		//mobile参数为空或者手机号码长度不为11或不为数字
            }

            string mobileHead = mobile.Substring(0, 3);
            if (mobileHead.Equals("134") || mobileHead.Equals("135") || mobileHead.Equals("136") || 
                mobileHead.Equals("137") || mobileHead.Equals("138") || mobileHead.Equals("139") || 
                mobileHead.Equals("150") || mobileHead.Equals("151") || mobileHead.Equals("152") || 
                mobileHead.Equals("157") || mobileHead.Equals("158") || mobileHead.Equals("159") ||
                mobileHead.Equals("182") || mobileHead.Equals("183") || mobileHead.Equals("184") || 
                mobileHead.Equals("187") || mobileHead.Equals("188"))
            {
                retType = 1;	//中国移动
            }
            else if (mobile.Substring(0, 3).Equals("130") || mobile.Substring(0, 3).Equals("131") || mobile.Substring(0, 3).Equals("132") ||
                     mobile.Substring(0, 3).Equals("155") || mobile.Substring(0, 3).Equals("156") || mobile.Substring(0, 3).Equals("185") ||
                     mobile.Substring(0, 3).Equals("186"))
            {
                retType = 2;	//中国联通
            }
            else if (mobile.Substring(0, 3).Equals("133") || mobile.Substring(0, 3).Equals("153") || 
                     mobile.Substring(0, 3).Equals("180") || mobile.Substring(0, 3).Equals("181") || mobile.Substring(0, 3).Equals("189"))
            {
                retType = 3;	//中国电信
            }
            else 
            {
                retType = -1;	//未知运营商
            }
            return retType;
        }        
    }
}
