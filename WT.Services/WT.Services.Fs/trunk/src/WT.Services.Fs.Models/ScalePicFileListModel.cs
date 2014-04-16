using WT.Components.Common.Models;
using System.Collections.Generic;

namespace WT.Services.Fs.Models
{
    public class ScalePicFileListModel:ResultModel
    {
        public Dictionary<string,string> picUrls { get; set; }
    }
}
