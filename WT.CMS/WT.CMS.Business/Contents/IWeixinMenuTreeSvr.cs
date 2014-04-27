using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT.CMS.Models.Contents;

namespace WT.CMS.Business.Contents
{
    public interface IWeixinMenuTreeSvr
    {
        /// <summary>
        /// 获取指定菜单列表
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<MenuItem> GetMenuItemsByParentid(int parentid, int status);
    }
}
