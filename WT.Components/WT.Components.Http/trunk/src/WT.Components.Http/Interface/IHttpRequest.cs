using System.Collections.Generic;
using WT.Components.Http.Models;

namespace WT.Components.Http.Interface
{
    public interface IHttpRequest
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        HttpMothed HttpMothed { get; set; }

        int Timeout { get; set; }

        /// <summary>
        /// 头部信息列表 -- 只读属性
        /// </summary>
        IList<Parameter> HeaderParameters { get;  }
        /// <summary>
        /// Form 参数列表  --  只读属性
        /// </summary>
        IList<Parameter> FormParameters { get;  }
        /// <summary>
        /// Query 参数列表  --  只读属性
        /// </summary>
        IList<Parameter> QueryParameters { get;  }
        /// <summary>
        /// cookie 参数列表  -- 只读属性
        /// </summary>
        IList<Parameter> CookieParameters { get;  }
        /// <summary>
        /// 文件参数列表  
        /// </summary>
        IList<FileParameter> FileParameterList { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        IList<Parameter> Parameters { get; set; }
        /// <summary>
        /// 请求地址信息
        /// </summary>
        string AddressUrl { get; set; }
        /// <summary>
        /// 是否存在文件
        /// </summary>
        bool HasFile { get; }

        /// <summary>
        /// 自定义内容实体，键值对 key=value&key1=value1  或者自定义内容格式
        /// eg:当上传文件时，无法自定义内容
        /// </summary>
        string CustomBody { get; set; }
    }
}
