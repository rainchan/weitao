using System.Collections.Generic;
using System.Linq;
using WT.Components.Http.Interface;

namespace WT.Components.Http.Models
{
    public class HttpRequest:IHttpRequest
    {
        public HttpRequest()
        {
            Parameters = new List<Parameter>();
            FileParameterList = new List<FileParameter>();
        }
        /// <summary>
        /// form 参数列表
        /// </summary>
        public IList<Parameter> FormParameters { get { return GetParameters(ParameterType.Form);} }
        /// <summary>
        /// header 参数列表
        /// </summary>
        public IList<Parameter> HeaderParameters { get { return GetParameters(ParameterType.Header); } }
        /// <summary>
        /// Query 参数列表
        /// </summary>
        public IList<Parameter> QueryParameters { get { return GetParameters(ParameterType.Query); } }
        /// <summary>
        /// Cookie 参数列表
        /// </summary>
        public IList<Parameter> CookieParameters { get { return GetParameters(ParameterType.Cookie); } }
        /// <summary>
        /// 文件参数列表
        /// </summary>
        public IList<FileParameter> FileParameterList { get; set; }
        /// <summary>
        /// 请求地址信息
        /// </summary>
        public string AddressUrl { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMothed HttpMothed { get; set; }

        public int Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// 是否存在文件
        /// </summary>
        public bool HasFile
        {
            get
            {
                return FileParameterList.Any();
            }
        }
        /// <summary>
        /// 参数列表
        /// </summary>
        public IList<Parameter> Parameters { get; set; }
        private IList<Parameter> GetParameters(ParameterType type)
        {
            return this.Parameters.Where(p => p.Type == type).ToList() ?? new List<Parameter>();
        }


        /// <summary>
        /// 自定义内容实体，键值对 key=value&key1=value1 或者自定义内容格式
        /// eg:当上传文件时，无法自定义内容
        /// </summary>
        public string CustomBody { get; set; }        
    }
}
