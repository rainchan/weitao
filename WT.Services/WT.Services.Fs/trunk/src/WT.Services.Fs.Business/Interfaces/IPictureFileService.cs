using System.Collections.Generic;
using WT.Services.Fs.Models;

namespace WT.Services.Fs.Business.Interfaces
{
    /// <summary>
    /// 文件处理服务
    /// </summary>
    public interface IPictureFileService
    {
        /// <summary>
        /// 获取一张指定尺寸的图片
        /// </summary>
        /// <param name="file_guid">原始图片guid，用来获取原始图片信息</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>返回指定尺寸的图片url地址</returns>
        AsyncFileDownloadResponse ScalePictureFile(string file_guid, int width, int height);

        /// <summary>
        /// 获取所有指定尺寸的图片列表
        /// </summary>
        /// <param name="file_guid">原始图片guid，用来获取原始图片信息</param>
        /// <param name="sizes">文件尺寸列表，格式如"["100_50","50_50"]"</param>
        /// <returns>返回指定尺寸的图片url地址列表</returns>
        ScalePicFileListModel ScalePictureFile(string file_guid, List<string> sizes);
    }
}
