using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{
    [Route("/fs/fileupload", "POST")]
    public class FileUploadRequest:BaseModel
    {
        public string file_name { get; set; }
        public long file_size { get; set; }
        public byte[] file_buffer { get; set; }
    }
}