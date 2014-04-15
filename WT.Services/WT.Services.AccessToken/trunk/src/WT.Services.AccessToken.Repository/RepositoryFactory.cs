using WT.Services.AccessToken.Models;

namespace WT.Services.AccessToken.Repository
{
    public class RepositoryFactory
    {        
        /// <summary>
        /// 获取微信AccessToken存储器
        /// </summary>
        /// <returns></returns>
        public static IRepository<WeiXinAccessTokenItem> GetWeiXinAccessTokenRepository()
        {
            return WeiXinAccessTokenRepository.Instance;
        }
    }
}
