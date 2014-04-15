using System;
using System.IO;
using System.Text;
using WT.Components.Common.Utility;

//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    public class TrackerClient
    {
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(TrackerClient));
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static int getUnixTime(DateTime datetime)
        {
            return (int)(datetime.ToUniversalTime() - epoch).TotalSeconds;
        }

        public static StorageServer GetStorageServer()
        {
            StorageServer server = null;
            FdfsConnection conn = null;

            try
            {
                conn = ConnectionManager.Instance.GetConnection(LinkType.Tracker);

                byte[] header;
                byte cmd = Protocol.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE;

                header = Protocol.PackHeader(cmd, 0, (byte)0);

                //FdfsPerfCounter.Instance.NumberOfSendPendingTracker.Increment();
                conn.Write(header);
                //FdfsPerfCounter.Instance.NumberOfSendPendingTracker.Decrement();

                Stream stream = conn.GetStream();

                //FdfsPerfCounter.Instance.NumberOfReceivePendingTracker.Increment();
                ReceivedPackage pkg = Protocol.PackPackage(string.Empty,ParsePackageView.Tracker,stream, Protocol.TRACKER_PROTO_CMD_RESP, Protocol.TRACKER_QUERY_STORAGE_STORE_BODY_LEN);
                //FdfsPerfCounter.Instance.NumberOfReceivePendingTracker.Decrement();

                if (pkg.ErrorCode != 0)
                {
                    throw new Exception(string.Format("GetStorageServer();pkg.ErrorCode:{0}", pkg.ErrorCode));
                }
                // to do
                //Encoding encode = Encoding.GetEncoding("ISO8859-1");
                LogUtil.Info(string.Format("TrackerClient GetStorageServer errorCode:{0}", pkg.ErrorCode));

                Encoding encode = Encoding.GetEncoding("UTF-8");
                string groupname = encode.GetString(pkg.Body, 0, Protocol.FDFS_GROUP_NAME_MAX_LEN - 1).Trim('\0');
                string ip = encode.GetString(pkg.Body, Protocol.FDFS_IPADDR_SIZE, Protocol.FDFS_IPADDR_SIZE).Trim('\0');
                int port = (int)Protocol.ConvertBuffToLong(pkg.Body, Protocol.FDFS_GROUP_NAME_MAX_LEN
                                + Protocol.FDFS_IPADDR_SIZE - 1);
                byte store_path = pkg.Body[Protocol.TRACKER_QUERY_STORAGE_STORE_BODY_LEN - 1];

                server = new StorageServer() { IP = ip, Port = port, StorePath = store_path };
                ConnectionManager.Instance.PutBackConnection(LinkType.Tracker, conn.RemoteEp, conn);

                //_tracing.InfoFmt("GetStorageServer:IP = {0}, Port = {1}, StorePath = {2}", ip, port, store_path);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("TrackerClient GetStorageServer error:{0}", ex.Message));

                //_tracing.ErrorFmt(ex, "GetStorageServer()");
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return server;
        }

        public static StorageServer GetStorageServer(string group, string fileName)
        {
            ServerInfo[] servers = GetStorages(Protocol.TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE, group, fileName);

            if (servers == null)
            {
                return null;
            }
            else
            {

               string ipVaild = servers[0].IP.Trim('\0');

               return new StorageServer() { IP = ipVaild, Port = servers[0].Port };
            }
        }
        public static ServerInfo[] GetStorages(byte cmd, string groupName, string filename)
        {
            ServerInfo[] servers = null;
            FdfsConnection conn = null;

            try
            {
                conn = ConnectionManager.Instance.GetConnection(LinkType.Tracker);

                byte[] header;
                byte[] bFileName;
                byte[] bGroupName;
                byte[] bs;
                int len;
                String ip_addr;
                int port;

                Encoding encode = Encoding.GetEncoding("ISO8859-1");
                bs = encode.GetBytes(groupName);
                bGroupName = new byte[Protocol.FDFS_GROUP_NAME_MAX_LEN];
                bFileName = encode.GetBytes(filename);

                if (bs.Length <= Protocol.FDFS_GROUP_NAME_MAX_LEN)
                {
                    len = bs.Length;
                }
                else
                {
                    len = Protocol.FDFS_GROUP_NAME_MAX_LEN;
                }

                Array.Copy(bs, 0, bGroupName, 0, len);

                header = Protocol.PackHeader(cmd, Protocol.FDFS_GROUP_NAME_MAX_LEN + bFileName.Length, (byte)0);
                byte[] wholePkg = new byte[header.Length + bGroupName.Length + bFileName.Length];
                Array.Copy(header, 0, wholePkg, 0, header.Length);
                Array.Copy(bGroupName, 0, wholePkg, header.Length, bGroupName.Length);
                Array.Copy(bFileName, 0, wholePkg, header.Length + bGroupName.Length, bFileName.Length);

                //FdfsPerfCounter.Instance.NumberOfSendPendingTracker.Increment();
                conn.Write(wholePkg);
                //FdfsPerfCounter.Instance.NumberOfSendPendingTracker.Decrement();
                Stream stream = conn.GetStream();
                //FdfsPerfCounter.Instance.NumberOfReceivePendingTracker.Increment();
                ReceivedPackage pkgInfo = Protocol.PackPackage(string.Empty, ParsePackageView.Tracker, stream, Protocol.TRACKER_PROTO_CMD_RESP, -1);
                //FdfsPerfCounter.Instance.NumberOfReceivePendingTracker.Decrement();

                if (pkgInfo.ErrorCode != 0)
                {
                    throw new Exception(string.Format("GetStorageServer();cmd:{0},groupName:{1},filename:{2};ErrorCode:{3}", cmd, groupName, filename, pkgInfo.ErrorCode));

                }

                if (pkgInfo.Body.Length < Protocol.TRACKER_QUERY_STORAGE_FETCH_BODY_LEN)
                {
                    throw new IOException("Invalid body length: " + pkgInfo.Body.Length);
                }

                if ((pkgInfo.Body.Length - Protocol.TRACKER_QUERY_STORAGE_FETCH_BODY_LEN) % (Protocol.FDFS_IPADDR_SIZE - 1) != 0)
                {
                    throw new IOException("Invalid body length: " + pkgInfo.Body.Length);
                }

                int server_count = 1 + (pkgInfo.Body.Length - Protocol.TRACKER_QUERY_STORAGE_FETCH_BODY_LEN) / (Protocol.FDFS_IPADDR_SIZE - 1);

                ip_addr = encode.GetString(pkgInfo.Body, Protocol.FDFS_GROUP_NAME_MAX_LEN, Protocol.FDFS_IPADDR_SIZE - 1).Trim();
                int offset = Protocol.FDFS_GROUP_NAME_MAX_LEN + Protocol.FDFS_IPADDR_SIZE - 1;

                port = (int)Protocol.ConvertBuffToLong(pkgInfo.Body, offset);
                offset += Protocol.TRACKER_PROTO_PKG_LEN_SIZE;

                servers = new ServerInfo[server_count];
                servers[0] = new ServerInfo() { IP = ip_addr, Port = port };
                for (int i = 1; i < server_count; i++)
                {
                    servers[i] = new ServerInfo() { IP = encode.GetString(pkgInfo.Body, offset, Protocol.FDFS_IPADDR_SIZE - 1).Trim(), Port = port };
                    offset += Protocol.FDFS_IPADDR_SIZE - 1;
                }

                ConnectionManager.Instance.PutBackConnection(LinkType.Tracker, conn.RemoteEp, conn);
            }
            catch (Exception ex)
            {
                //_tracing.ErrorFmt(ex, "GetStorageServer();cmd:{0},groupName:{1},filename:{2}",cmd,groupName,filename);
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return servers;
        }
    }
}
