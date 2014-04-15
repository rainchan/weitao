using System;
using Newtonsoft.Json;

namespace WT.Components.Common.Utility
{
    public class JsonUtil<T>
    {
        public static string ToJson(T model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T FromJosn(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                throw new ArgumentException(string.Format("json转化到 {0} 实体出错，json值详细：{1}", typeof(T).Name, json));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static M FromJosn<M>(string json, M value)
        {
            try
            {
                return JsonConvert.DeserializeAnonymousType(json, value);
            }
            catch
            {
                throw new ArgumentException(string.Format("json转化到 {0} 实体出错，json值详细：{1}", typeof(T).Name, json));
            }
        }
    }
}
