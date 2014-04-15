using System.Collections.Specialized;
using WT.Components.Common.Utility;

namespace WT.Components.Database
{
    public abstract class DbInstance
    {
        protected StringDictionary ConnectionStrings { get; private set; }

        protected string ConnectionString { get; set; }

        public DbInstance()
        {
            ConnectionStrings = ConfigUtil.GetConnectionStrings();
        }
    }
}
