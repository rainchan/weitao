using WT.Components.Common.Models;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// 文件同步下载响应结果
    /// </summary>
    public class FileDownloadResponse : ResultModel
    {
        public string File_name { get; set; }
        public byte[] File_buffer { get; set; }    

    }
}
