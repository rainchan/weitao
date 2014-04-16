using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{
    [Route("/fs/pic")]
    public class FSScalePicFileRequest:BaseModel
    {
        public string file_guid { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}