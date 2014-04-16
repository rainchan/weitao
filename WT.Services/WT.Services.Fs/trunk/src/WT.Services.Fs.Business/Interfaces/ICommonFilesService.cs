using WT.Services.Fs.Models;

namespace WT.Services.Fs.Business.Interfaces
{
    /// <summary>
    /// 公共文件服务
    /// </summary>
    public interface ICommonFilesService
    {
        /// <summary>
        /// 异步文件流上传
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns>上传响应结果</returns>
        AsyncsFileUploadResponse AsynFileUpload(string fileName, byte[] bytes);

        /// <summary>
        /// 异步文件流上传，指定guid
        /// </summary>
        /// <param name="guid">文件guid，唯一标识符</param>
        /// <param name="fileName">文件名</param>
        /// <param name="bytes">文件二进制流</param>
        /// <returns>上传响应结果</returns>
        AsyncsFileUploadResponse AsynFileUploadWithGuid(string guid, string fileName, byte[] bytes);

        /// <summary>
        /// 文件下载，返回文件地址
        /// </summary>
        /// <param name="file_guid">文件guid</param>
        /// <returns>下载响应结果</returns>
        AsyncFileDownloadResponse AsynFileDownload(string file_guid);

         /// <summary>
        /// amr转MP3文件服务特别使用，将amr文件上传至fdfs
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileGuid">文件guid</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns>上传响应结果</returns>
        AsyncsFileUploadResponse AsynAmrFileUpload(string fileName, string fileGuid, byte[] bytes);

        /// <summary>
        /// 文件字节流上传
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="file_size">文件大小</param>
        /// <param name="file_buffer">文件字节流</param>
        /// <returns>上传响应结果</returns>
        FileUploadResponse FileUpload(string fileName, long file_size, byte[] file_buffer);

        /// <summary>
        /// 下载文件字节流
        /// </summary>
        /// <param name="file_guid">guid</param>
        /// <returns>上传响应结果</returns>
        FileDownloadResponse FileDownload(string file_guid);
    }
}
