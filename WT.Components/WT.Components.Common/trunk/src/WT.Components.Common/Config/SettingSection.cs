using System.Configuration;

namespace WT.Components.Common.Config
{
    public class SettingSection : ConfigurationSection
    {
        [ConfigurationProperty("settings", IsDefaultCollection = true)]
        public KeyValueConfigurationCollection Settings
        {
            get
            {
                return (KeyValueConfigurationCollection)this["settings"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetSetting(string key)
        {
            if (Settings != null && Settings[key] != null)
            {
                return Settings[key].Value;
            }

            return "";
        }
    }
}
