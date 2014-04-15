using System.Web;

namespace WT.Components.Common.Helpers
{
    public sealed class PathHelper
    {
        /// <summary>
        /// 得到Server.MapPath下的全路径
        /// </summary>
        /// <param name="relativeUrl">相对路径</param>
        /// <returns></returns>
        public static string MapPath(string relativeUrl)
        {
            return HttpContext.Current.Server.MapPath(relativeUrl);
        }
    }
}
