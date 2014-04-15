using WT.Components.Authorisation.Model;
using WT.Components.Common.Config;
using WT.Components.Http;
using WT.Components.Http.Interface;
using WT.Components.Http.Models;
using WT.Components.Common.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT.Components.Authorization
{
    public class AutonaviAuthorisationService
    {
        private static SettingSection _accesstokenSection = null;

        /// <summary>
        /// 配置节信息获取
        /// </summary>
        public static SettingSection AccesstokenSection
        {
            get
            {
                if (_accesstokenSection == null)
                {
                    _accesstokenSection = (SettingSection)ConfigurationManager.GetSection("accesstoken");
                }

                return _accesstokenSection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string LoginUrl
        {
            get
            {
                if (AccesstokenSection != null)
                {
                    return AccesstokenSection.GetSetting("autonaviLogin");
                }

                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string LogoutUrl
        {
            get
            {
                if (AccesstokenSection != null)
                {
                    return AccesstokenSection.GetSetting("autonaviLogout");
                }

                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AutonaviLogOnResult AutonaviLogin()
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();
            fields.Add("app_id", "1");
            fields.Add("app_version", "0");
            fields.Add("os_type", "0");
            fields.Add("device_id", "0");

            IHttpRequest request = ConfigHttpRequest(LoginUrl, fields, null, HttpMothed.POST, ParameterType.Form);
            IHttpResponse response = HttpUtil.ExecuteSync(request);
            string jsondata = Encoding.GetEncoding("UTF-8").GetString(response.RawBytes);
            return  JsonUtil<AutonaviLogOnResult>.FromJosn(jsondata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool AutonaviLogout(long version)
        {
            bool res = true;
            Dictionary<string, object> fields = new Dictionary<string, object>();
            fields.Add("app_id", "1");
            fields.Add("app_version", "0");
            fields.Add("os_type", "0");
            fields.Add("device_id", "0");
            fields.Add("version", version);

            IHttpRequest request = ConfigHttpRequest(LogoutUrl, fields, null, HttpMothed.POST, ParameterType.Form);
            IHttpResponse response = HttpUtil.ExecuteSync(request);
            string jsondata = Encoding.GetEncoding("UTF-8").GetString(response.RawBytes);
            Dictionary<string, string> dic = JsonUtil<Dictionary<string, string>>.FromJosn(jsondata);

            if (string.IsNullOrEmpty(dic["ret"]) || dic["ret"] != "200")
            {
                res = false;
            }

            return res;
        }

        protected static IHttpRequest ConfigHttpRequest(string url, Dictionary<string, object> fields, Dictionary<string, object> headers, HttpMothed method, ParameterType parameterType)
        {

            IHttpRequest request = new HttpRequest();

            request.HttpMothed = method;
            IList<Parameter> parameters = new List<Parameter>();
            if (fields != null && fields.Count > 0)
            {
                foreach (var item in fields)
                {
                    Parameter p = new Parameter();
                    p.Name = item.Key;
                    p.Value = item.Value;
                    p.Type = parameterType;
                    parameters.Add(p);
                }
            }
            request.Parameters = parameters;

            parameters = new List<Parameter>();
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    Parameter p = new Parameter();
                    p.Name = item.Key;
                    p.Value = item.Value;
                    p.Type = ParameterType.Header;
                    request.HeaderParameters.Add(p);
                }
            }

            request.AddressUrl = url;
            return request;
        }
    }
}
