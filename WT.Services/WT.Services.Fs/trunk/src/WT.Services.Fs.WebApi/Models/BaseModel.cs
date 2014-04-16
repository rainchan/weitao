using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WT.Services.Fs.WebApi.Models
{
    public class BaseModel
    {
        public string app_version { get; set; }
        public int os_type { get; set; }
        public string device_id { get; set; }
        public int app_id { get; set; }
    }
}