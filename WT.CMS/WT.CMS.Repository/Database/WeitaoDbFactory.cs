using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT.CMS.Repository.Database
{
    public class WeitaoDbFactory
    {
        private const string db_conn_write_key = "conn_weitaoDb_write";
        private const string db_conn_read_key = "conn_weitaoDb_read";


        public static IWeixinMenuTreeDA CreateReadMenuTree()
        {
            return new WeixinMenuTreeDA(db_conn_read_key);
        }

        public static IWeixinMenuTreeDA CreateWriteMenuTree()
        {
            return new WeixinMenuTreeDA(db_conn_write_key);
        }
    }
}
