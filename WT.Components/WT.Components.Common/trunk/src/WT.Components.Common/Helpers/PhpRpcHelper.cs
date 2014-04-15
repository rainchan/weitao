using org.phprpc;
using System;
using System.Collections.Generic;

namespace WT.Components.Common.Helpers
{
    public interface IPhprpc
    {
        string data(string api, Dictionary<string, string> args);
    }

    public class PhpRpcHelper
    {
        public static string RpcCall(string url, string method, Dictionary<string, string> args)
        {
            string result = string.Empty;
            try
            {
                PHPRPC_Client rpc = new PHPRPC_Client(url);
                IPhprpc p = (IPhprpc)rpc.UseService(typeof(IPhprpc));
                result = p.data(method, args);
            }
            catch(Exception ex)
            {
                string error = string.Format("Exception in RpcCall():{0}, url = {1}, method={2}, args={3}", ex.ToString(), url, method, args.ToString());
            }
            return result;
        }
    }
}
