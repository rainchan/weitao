using System;
using System.IO;

namespace WT.Components.Http.Models
{
    public struct FileParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">参数的名称</param>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        public FileParameter(string name, byte[] data, string filename, string contentType)
        {
            this.Writer = s => s.Write(data, 0, data.Length);
            this.FileName = filename;
            this.ContentType = contentType;
            this.ContentLength = data.LongLength;
            this.Name = name;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 文件长度
        /// </summary>
        public long ContentLength;
        /// <summary>
        /// 读写操作流
        /// </summary>
        public Action<Stream> Writer;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType;
    }
}
