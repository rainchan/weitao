using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace WT.Components.Common.Utility
{
    public static class ConfigUtil
    {
        public static NameValueCollection AppSettings { get; private set; }

        static ConfigUtil()
        {
            AppSettings = ConfigurationManager.AppSettings;
        }

        public static string GetAppSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public static StringDictionary GetConnectionStrings()
        {
            StringDictionary reval = new StringDictionary();
            ConnectionStringSettingsCollection StrConnColl = ConfigurationManager.ConnectionStrings;
            //#if DEBUG
            for (int i = 0; i < StrConnColl.Count; i++)
            {
                reval.Add(StrConnColl[i].Name, StrConnColl[i].ConnectionString);

            }
            //#else
            //            for (int i = 0; i < StrConnColl.Count; i++)
            //            {
            //               reval.Add(StrConnColl[i].Name,CryptoHelper.Decrypt(StrConnColl[i].ConnectionString,StrConnColl[i].Name));
            //            }
            //#endif

            return reval;

        }

        public static string GetSectionValue(string section, string key)
        {
            if (!string.IsNullOrEmpty(section))
            {
                var table = ConfigurationManager.GetSection(section) as Hashtable;
                if (table != null) return table[key] as string;
            }
            throw new ArgumentNullException();
        }
    }
}
