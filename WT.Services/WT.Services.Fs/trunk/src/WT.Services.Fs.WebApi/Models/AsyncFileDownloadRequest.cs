using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{

    [Route("/fs/asyncfiledownload", "POST")]
    public class AsyncFileDownloadRequest:BaseModel
    {
        public string file_guid { get; set; }
    }
}