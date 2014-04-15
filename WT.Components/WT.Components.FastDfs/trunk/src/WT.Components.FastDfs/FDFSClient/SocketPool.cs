using System;
using System.Net.Sockets;

//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    /// <summary>
    /// 不实现socket上的连接池，容易出问题
    /// </summary>
    public static class SocketPool
    {
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(SocketPool));

        private static readonly AddressFamily IpFamily = AddressFamily.InterNetwork;

        public static void Initialize()
        {
        }

        public static Socket GetOne()
        {
            Socket conn = null;
            {
                conn = new Socket(IpFamily, SocketType.Stream, ProtocolType.Tcp);
                //FdfsPerfCounter.Instance.NumberOfSocketCreation.Increment();
                //FdfsPerfCounter.Instance.RateOfSocketCreation.Increment();
            }
            return conn;
        }

        public static void Close(Socket conn, bool force)
        {
            if (conn == null)
                return;

            try
            {
                if (conn.Connected)
                    conn.Shutdown(SocketShutdown.Both);

            }
            catch(Exception ex)
            {
                //_tracing.ErrorFmt(ex, "Socket Close");
            }

            finally
            {
                conn.Close();
                //FdfsPerfCounter.Instance.NumberOfSocketClose.Increment();
                //FdfsPerfCounter.Instance.RateOfSocketClose.Increment();
            }
        }
    }
}
