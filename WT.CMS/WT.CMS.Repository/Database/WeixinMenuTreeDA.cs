using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT.CMS.Repository.Entity;
using WT.Components.Common.Utility;

using ServiceStack.OrmLite;


namespace WT.CMS.Repository.Database
{
    public class WeixinMenuTreeDA: WeitaoDbInstance, IWeixinMenuTreeDA
    {
         public WeixinMenuTreeDA() : base() { 
        
        }

         public WeixinMenuTreeDA(string conn)
             : base(conn)
         { 
        
        }

         public List<WeixinMenuTreeEntity> GetMenus(int parentid = 0, int status = 1)
         {
             List<WeixinMenuTreeEntity> menus = null;
             try
             {
                 using (IDbConnection dbCmd = DBFactory.OpenDbConnection())
                 {
                     string sql = "select Id,parent_id, menu_name, command, status, create_time, last_changed_time "
                        + "from weixin_menu_tree where parent_id = {0} and status = {1}".Params(parentid, status);
                     menus = dbCmd.Query<WeixinMenuTreeEntity>(sql);
                 }
             }
             catch (Exception e)
             {
                 string err = string.Format("DataBase Exception in get WeixinMenuTreeEntities: {0}", e.ToString());
                 LogUtil.Error(err);
             }

             return menus;
         }

         public int CreateNewMenuItem(WeixinMenuTreeEntity item)
         {
             if (item == null)
             {
                 return 0;
             }

             try
             {
                 using (IDbConnection dbcmd = DBFactory.OpenDbConnection())
                 {
                     string sql = "insert into weiin_menu_tree(menu_name, command, status, create_time, last_changed_time) "
                                + "values({0}, {1}, {2}, {3}, {4})".Params(item.menu_name, item.command, item.create_time, item.last_changed_time);
                     return dbcmd.ExecuteSql(sql);
                 }
             }
             catch (Exception e)
             {
                 string err = string.Format("DataBase Exception in create WeixinMenuTreeEntity(): {0}", e.ToString());
                 LogUtil.Error(err);
                 throw;
             }
         }

         public int UpdateMenuItem(WeixinMenuTreeEntity item)
         {
             if (item == null)
                 return 0;
             try
             {
                 using (IDbConnection dbcmd = DBFactory.OpenDbConnection())
                 {
                     string sql = "update weixin_menu_tree set menu_name ={0}, command = {1}, last_changed_time = {2} where Id={3}".Params
                         (item.menu_name, item.command, item.Id, item.last_changed_time);
                     return dbcmd.ExecuteSql(sql);
                 }
             }
             catch (Exception e)
             {
                 string err = string.Format("DataBase Exception in create UpdateMenuItem(): {0}", e.ToString());
                 LogUtil.Error(err);
                 throw;
             }
             
         }

         public int DelMenuItem(int id)
         {
             try
             {
                 using (IDbConnection dbcmd = DBFactory.OpenDbConnection())
                 {
                     string sql = "update weixin_menu_tree set status =0 where Id={0} or parent_id ={0}".Params(id);
                     return dbcmd.ExecuteSql(sql);
                 }
             }
             catch (Exception e)
             {
                 string err = string.Format("DataBase Exception in create DelMenuItem(): {0}", e.ToString());
                 LogUtil.Error(err);
                 throw;
             }
         }
    }
}
