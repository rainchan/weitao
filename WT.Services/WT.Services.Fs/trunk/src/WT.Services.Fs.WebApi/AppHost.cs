using ServiceStack.WebHost.Endpoints;
using WT.Components.Common.Utility;
using System.Collections;
using System.Configuration;
using WT.Components.FastDfs;
using WT.Services.Fs.WebApi;

[assembly: WebActivator.PreApplicationStartMethod(typeof(AppHost), "Start")]
namespace WT.Services.Fs.WebApi
{
    public class AppHost
        : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services
            : base("StarterTemplate ASP.NET Host", typeof(CommonFileServiceHost).Assembly) {

                Hashtable storagerTable = ConfigurationManager.GetSection("FdfsStorages") as Hashtable;
                Hashtable trackerTable = ConfigurationManager.GetSection("FdfsTrackers") as Hashtable;
                FSManager.Initialize(trackerTable, storagerTable);
        }

        public override void Configure(Funq.Container container)
        {
            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            this.RequestFilters.Add((httpReq, httpRes, requestDto) =>
            {
                var app_id = httpReq.FormData["app_id"];
                if (string.IsNullOrEmpty(app_id))
                {

                    httpRes.Write("{\"ret\":400,\"message\":\" 参数app_id 不能为空\"}");
                    httpRes.Close();
                }
            });
        }


        public static void Start()
        {
            LogUtil.Info("AppHost start");
            new AppHost().Init();
        }
    }
}