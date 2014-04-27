using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT.CMS.Repository.Entity;

namespace WT.CMS.Repository.Database
{
    public interface IWeixinMenuTreeDA
    {
        /// <summary>
        /// 获取指定状态下，父id下所有子项
        /// 默认选中所有激活状态
        /// 
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<WeixinMenuTreeEntity> GetMenus(int parentid = 0, int status = 1);

        /// <summary>
        /// 新建新的菜单子项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int CreateNewMenuItem(WeixinMenuTreeEntity item);

        /// <summary>
        /// 更新菜单子项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateMenuItem(WeixinMenuTreeEntity item);

        /// <summary>
        /// 删除指定项及其子项所有的菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DelMenuItem(int id);

    }
}
