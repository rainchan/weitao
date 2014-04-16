using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{
    [Route("/fs/asyncpics")]
    public class FSAsyncScalePicFileListRequest
    {
        public string file_guid { get; set; }
        public string sizes { get; set; }
    }
}