using WT.Components.Database;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace WT.Services.Fs.Repository.Database
{
    public class FSDbInstance:DbInstance
    {
        protected OrmLiteConnectionFactory dbFactory { get; set; }

        public FSDbInstance()
        {
            ConnectionString = ConnectionStrings["conn_snsDb"];
            dbFactory = new OrmLiteConnectionFactory(ConnectionString, MySqlDialectProvider.Instance);
        }

        /// <summary>
        /// 根据给定的连接串创建数据库实例
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        public FSDbInstance(string connKey)
        {
            ConnectionString = ConnectionStrings[connKey];
            dbFactory = new OrmLiteConnectionFactory(ConnectionString, MySqlDialectProvider.Instance);
        }
    }
}
