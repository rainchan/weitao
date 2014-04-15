using WT.Components.Common.Models;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace WT.Services.AccessToken.WebApi
{
    public class AccessTokenAppHost: AppHostBase
    {
        public AccessTokenAppHost()
            : base("AccessToken App Host", typeof(AccessTokenAppHost).Assembly)
        { }

        public override void Configure(Funq.Container container)
        {
            //ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            this.RequestFilters.Add((httpReq, httpRes, requestDto) =>
            {
                var app_id = httpReq.FormData["app_id"];
                if (string.IsNullOrEmpty(app_id))
                {
                    ResultModel result = new ResultModel();
                    result.ret = 300;
                    result.message = "参数app_id 不能为空";
                    httpRes.Write(JsonSerializer.SerializeToString<ResultModel>(result));
                    httpRes.Close();
                }
            });
        }

        public static void Start()
        {
            new AccessTokenAppHost().Init();
        }
    }
}