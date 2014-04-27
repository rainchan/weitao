using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT.CMS.Models.Contents;
using WT.CMS.Repository.Database;
using WT.CMS.Repository.Entity;

namespace WT.CMS.Business.Contents
{
    public class WeixinMenuTreeSvr: IWeixinMenuTreeSvr
    {
        private static WeixinMenuTreeSvr service;
        public static WeixinMenuTreeSvr Current 
        {
            get { return service; }
        }

        static WeixinMenuTreeSvr()
        {
            service = new WeixinMenuTreeSvr();
        }

        private WeixinMenuTreeSvr()
        {
        }

        public List<MenuItem> GetMenuItemsByParentid(int parentid, int status)
        {
            List<MenuItem> items = new List<MenuItem>();

            IWeixinMenuTreeDA da = WeitaoDbFactory.CreateReadMenuTree();
            List<WeixinMenuTreeEntity> treeEntities = da.GetMenus(parentid, status);
            foreach (WeixinMenuTreeEntity item in treeEntities)
            {
                MenuItem menu = new MenuItem();
                menu.Id = item.Id;
                menu.ParentId = item.parent_id;
                menu.Name = item.menu_name;
                menu.Command = item.command;

                items.Add(menu);
            }

            return items;
        }
    }
}
