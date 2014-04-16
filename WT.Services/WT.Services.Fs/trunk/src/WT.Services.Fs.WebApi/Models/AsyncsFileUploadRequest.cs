using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{
    [Route("/fs/asyncfileupload", "POST")]
    public class AsyncsFileUploadRequest:BaseModel
    {
        public string file_name { get; set; }
        public long file_size { get; set; }
    }
}