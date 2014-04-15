namespace WT.Components.Common.Models
{
    /// <summary>
    /// 第三方平台类型
    /// </summary>
    public enum PlatFormType
    {
        /// <summary>
        /// 新浪微博
        /// </summary>
        Sina = 2,
        /// <summary>
        /// 腾讯微博
        /// </summary>
        Tencent = 3,
        /// <summary>
        /// QQ空间
        /// </summary>
        Qzone = 4,
        /// <summary>
        /// 开心网
        /// </summary>
        Kaixin = 5,
        /// <summary>
        /// 人人
        /// </summary>
        Renren = 6,
        /// <summary>
        /// 豆瓣
        /// </summary>
        DouBan = 7
    }

    public enum AppIdType
    {
        /// <summary>
        /// 车托帮
        /// </summary>
        Ctb = 1,

        /// <summary>
        /// 微信路况
        /// </summary>
        Weixin = 2,

        /// <summary>
        /// 到哪啦
        /// </summary>
        Daona = 3,

        /// <summary>
        /// 查违章
        /// </summary>
        Cwz = 4,

        /// <summary>
        /// 查路况
        /// </summary>
        Clk = 5
    }

    public enum WeiboMsgType
    {
        /// <summary>
        /// 事件消息
        /// </summary>
        Event = 1,

        /// <summary>
        /// 纯文本类型私信和留言消息
        /// </summary>
        Text = 2,

        /// <summary>
        /// 图片类型私信和留言消息
        /// </summary>
        Image = 3,

        /// <summary>
        /// 位置类型私信消息
        /// </summary>
        Position = 4,

        /// <summary>
        /// 被@消息
        /// </summary>
        Mention = 5,
        
        /// <summary>
        /// 语音消息
        /// </summary>
        Voice = 6,

        /// <summary>
        /// 图文类型私信消息
        /// </summary>
        Articles = 7
    }
}
