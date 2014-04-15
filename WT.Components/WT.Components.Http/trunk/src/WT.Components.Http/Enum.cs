namespace WT.Components.Http
{
    public enum HttpMothed
    {
        /// <summary>
        /// 防止没有设置默认值，根据此值判断
        /// </summary>
        None,
        /// <summary>
        /// Get 
        /// </summary>
        GET = 2,
        /// <summary>
        /// post
        /// </summary>
        POST = 4

    }

    /// <summary>
    /// 返回的状态
    /// </summary>
    public enum ResponseStatus
    {
        None,
        Completed,
        Error,
        ErrorButResponse,
        TimedOut,
        Aborted
    }
    /// <summary>
    /// 平台参数类型
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// 属于url请求数据
        /// </summary>
        Query,
        /// <summary>
        /// form表单数据
        /// </summary>
        Form,
        /// <summary>
        /// 地址中的值    
        /// </summary>
        UrlSegment,
        /// <summary>
        /// 头部参数
        /// </summary>
        Header,
        /// <summary>
        /// cookie
        /// </summary>
        Cookie
    }
}
