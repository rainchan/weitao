using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WT.CMS.Business.Contents;
using WT.CMS.Models.Contents;

namespace WT.CMS.WebSite.Controllers.Content
{
    public class ContentMenuTreeController : Controller
    {
        //
        // GET: /ContentMenuTree/

        public ActionResult Index()
        {
            IWeixinMenuTreeSvr svr = WeixinMenuTreeSvr.Current;
            List<MenuItem> menus = svr.GetMenuItemsByParentid(0, 1);
            return View();
        }

    }
}
