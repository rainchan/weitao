using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT.Components.Database;

namespace WT.CMS.Repository.Database
{
    public class WeitaoDbInstance : DbInstance
    {
         protected OrmLiteConnectionFactory DBFactory { get; set; }

        public WeitaoDbInstance()
        {
            ConnectionString = ConnectionStrings["conn_Db"];
            DBFactory = new OrmLiteConnectionFactory(ConnectionString, MySqlDialectProvider.Instance);
        }

        /// <summary>
        /// 根据给定的连接串创建数据库实例
        /// </summary>
        /// <param name="connKey">链接字符串</param>
        public WeitaoDbInstance(string connKey)
        {
            ConnectionString = ConnectionStrings[connKey];
            DBFactory = new OrmLiteConnectionFactory(ConnectionString, MySqlDialectProvider.Instance);
        }
    }
}
