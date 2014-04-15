using System;
using System.IO;
using System.Text;
using System.Net;
using System.Diagnostics;
using WT.Components.Common.Utility;

//using Imps.Services.CloudFile.Config;
//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    public class StorageClient
    {
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(StorageClient));
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static int getUnixTime(DateTime datetime)
        {
            return (int)(datetime.ToUniversalTime() - epoch).TotalSeconds;
        }

        #region Appendix Function

        public static void Upload(StorageServer storage_server, string file_ext_name, long file_size, byte[] file_buffer,string fileBlockKey, Action<string[]> call_back)
        {
           
            FdfsConnection conn = null;
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(storage_server.IP), storage_server.Port);
                conn = ConnectionManager.Instance.GetConnection(LinkType.Storages, endPoint);
                LogUtil.Info(string.Format("StorageClient Upload conn {0} server endPoint {1}",conn.IsConnected, endPoint.Address));
                byte[] header;
                byte[] ext_name_bs;
                String new_group_name;
                String remote_filename;
                byte[] sizeBytes;
                byte[] hexLenBytes;
                int offset;
                long body_len;
                byte cmd;

                ext_name_bs = new byte[Protocol.FDFS_FILE_EXT_NAME_MAX_LEN];
                Encoding encode = Encoding.GetEncoding("UTF-8");
                if (file_ext_name != null && file_ext_name.Length > 0)
                {
                    byte[] bs = encode.GetBytes(file_ext_name);
                    int ext_name_len = bs.Length;
                    if (ext_name_len > Protocol.FDFS_FILE_EXT_NAME_MAX_LEN)
                    {
                        ext_name_len = Protocol.FDFS_FILE_EXT_NAME_MAX_LEN;
                    }
                    Array.Copy(bs, 0, ext_name_bs, 0, ext_name_len);
                }

                cmd = Protocol.STORAGE_PROTO_CMD_UPLOAD_FILE;
                sizeBytes = new byte[1 + 1 * Protocol.TRACKER_PROTO_PKG_LEN_SIZE];
                body_len = sizeBytes.Length + Protocol.FDFS_FILE_EXT_NAME_MAX_LEN + file_size;

                sizeBytes[0] = (byte)storage_server.StorePath;
                offset = 1;

                hexLenBytes = Protocol.ConvertLongToBuff(file_size);
                Array.Copy(hexLenBytes, 0, sizeBytes, offset, hexLenBytes.Length);

                header = Protocol.PackHeader(cmd, body_len, (byte)0);
                byte[] wholePkg = new byte[(int)(header.Length + body_len - file_size)];
                Array.Copy(header, 0, wholePkg, 0, header.Length);
                Array.Copy(sizeBytes, 0, wholePkg, header.Length, sizeBytes.Length);
                offset = header.Length + sizeBytes.Length;

                Array.Copy(ext_name_bs, 0, wholePkg, offset, ext_name_bs.Length);
                offset += ext_name_bs.Length;

                byte[] buffer = new byte[(int)(wholePkg.Length + file_buffer.Length)];
                Array.Copy(wholePkg, 0, buffer, 0, wholePkg.Length);
                Array.Copy(file_buffer, 0, buffer, wholePkg.Length, file_buffer.Length);

                //if (ConfigManager.Instance.DumpObjEnable)
                //_tracing.InfoFmt("upload storage_server:{0};file_ext_name:{1};file_size:{2};file_buffer_length:{3};fileBlockKey:{4};header:{5}", endPoint, file_ext_name, file_size, file_buffer.Length, fileBlockKey, ObjectHelper.DumpObject(header));

                Stopwatch watch = new Stopwatch();
                watch.Start();
                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Increment();
                conn.Write(buffer);
                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Decrement();
                Stream stream = conn.GetStream();
                watch.Stop();
                string info = string.Format("StorageClient.Upload():storage_server ={0}:{1}, file_size={2}, upload cost: {3}", storage_server.IP, storage_server.Port, file_size, watch.ElapsedMilliseconds);
                if (watch.ElapsedMilliseconds > 3000)
                    LogUtil.Error(info);
                else
                    LogUtil.Info(info);

                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Increment();
                ReceivedPackage pkgInfo = Protocol.PackPackage(fileBlockKey,ParsePackageView.Upload,stream, Protocol.STORAGE_PROTO_CMD_RESP, -1);
                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Decrement();

                if (pkgInfo == null)
                {
                    throw new Exception(string.Format("StorageClient.Upload(): resp pkg info is null;endPoint:{0};file_ext_name:{1};file_size:{2}", endPoint, file_ext_name, file_size));

                }

                if (pkgInfo.ErrorCode != 0)
                {
                    if (pkgInfo.Body == null)
                    {
                        // 对应那种返回文件块路径为空的情况
                       throw new Exception(string.Format("StorageClient.Upload(): ErrorCode = {0}, RespPkgBody is null, while headerLength = {1}, body.fileSizeBytesLength = {2}, body.fileExtBytesLength = {3}, body.fileContentBytesLength = {4}, totalSentRequestSize ={5}, Server[{6}:{7}] ", pkgInfo.ErrorCode, header.Length, sizeBytes.Length, ext_name_bs.Length, file_buffer.Length, buffer.Length, storage_server.IP, storage_server.Port));
                    }
                    else
                    {
                       throw new Exception(string.Format("StorageClient.Upload(): ErrorCode = {0}, RespPkgBody not null, while headerLength = {1}, body.fileSizeBytesLength = {2}, body.fileExtBytesLength = {3}, body.fileContentBytesLength = {4}, totalSentRequestSize ={5}, Server[{6}:{7}] ", pkgInfo.ErrorCode, header.Length, sizeBytes.Length, ext_name_bs.Length, file_buffer.Length, buffer.Length, storage_server.IP, storage_server.Port));
                    }

                }

                if (pkgInfo.Body.Length <= Protocol.FDFS_GROUP_NAME_MAX_LEN)
                {
                    //_tracing.ErrorFmt("StorageClient.Upload(): RespBodyLength=" + pkgInfo.Body.Length + " <= " + Protocol.FDFS_GROUP_NAME_MAX_LEN);
                    throw new Exception("body length: " + pkgInfo.Body.Length + " <= " + Protocol.FDFS_GROUP_NAME_MAX_LEN);
                }

                new_group_name = encode.GetString(pkgInfo.Body, 0, Protocol.FDFS_GROUP_NAME_MAX_LEN).Trim('\0');
                remote_filename = encode.GetString(pkgInfo.Body, Protocol.FDFS_GROUP_NAME_MAX_LEN, pkgInfo.Body.Length - Protocol.FDFS_GROUP_NAME_MAX_LEN);
                string[] results = new string[2];
                results[0] = new_group_name;
                results[1] = remote_filename;
                call_back(results);

                ConnectionManager.Instance.PutBackConnection(LinkType.Storages, endPoint, conn);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("StorageClient Upload error {0}",ex.ToString()));

                //_tracing.ErrorFmt(ex, "StorageClient.Upload():ip:{0};port:{1};file_ext_name:{1};file_size:{2}", storage_server.IP,storage_server.Port, file_ext_name, file_size);

                if (conn != null)
                {
                    conn.Close();
                }
                call_back(null);
            }
        }

        public static void Download(StorageServer storage_server, string group_name, string filename, long file_offset, long download_file_bytes_length, Action<byte[]> call_back)
        {

            FdfsConnection conn = null;

            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(storage_server.IP), storage_server.Port);
                conn = ConnectionManager.Instance.GetConnection(LinkType.Storages, endPoint);
                byte[] header;
                byte[] filename_bs;
                byte[] group_name_bs;
                byte[] sizeBytes;
                byte[] hexLenBytes;
                int offset;
                long body_len;
                byte cmd;

                Encoding encode = Encoding.GetEncoding("ISO8859-1");
                cmd = Protocol.STORAGE_PROTO_CMD_DOWNLOAD_FILE;
                sizeBytes = new byte[2 * Protocol.TRACKER_PROTO_PKG_LEN_SIZE + Protocol.FDFS_GROUP_NAME_MAX_LEN];

                hexLenBytes = Protocol.ConvertLongToBuff(file_offset);
                Array.Copy(hexLenBytes, 0, sizeBytes, 0, Protocol.TRACKER_PROTO_PKG_LEN_SIZE);

                hexLenBytes = Protocol.ConvertLongToBuff(0);
                Array.Copy(hexLenBytes, 0, sizeBytes, Protocol.TRACKER_PROTO_PKG_LEN_SIZE, Protocol.TRACKER_PROTO_PKG_LEN_SIZE);

                group_name_bs = encode.GetBytes(group_name);
                Array.Copy(group_name_bs, 0, sizeBytes, 2 * Protocol.TRACKER_PROTO_PKG_LEN_SIZE, group_name_bs.Length);

                filename_bs = encode.GetBytes(filename);
                body_len = sizeBytes.Length + filename_bs.Length;
                offset = 0;

                header = Protocol.PackHeader(cmd, body_len, (byte)0);
                byte[] wholePkg = new byte[(int)(header.Length + body_len)];
                Array.Copy(header, 0, wholePkg, 0, header.Length);
                Array.Copy(sizeBytes, 0, wholePkg, header.Length, sizeBytes.Length);

                offset = header.Length + sizeBytes.Length;

                Array.Copy(filename_bs, 0, wholePkg, offset, filename_bs.Length);

                //if (ConfigManager.Instance.DumpObjEnable)
                //    _tracing.InfoFmt("download storage_server:{0};group_name:{1};filename:{2};file_offset:{3};download_file_bytes_length:{4};header:{5}", endPoint,group_name,filename, file_offset, download_file_bytes_length, ObjectHelper.DumpObject(header));

                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Increment();
                conn.Write(wholePkg);
                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Decrement();

                Stream stream = conn.GetStream();

                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Increment();
                ReceivedPackage pkgInfo = Protocol.PackPackage(string.Format("storage_server:{0};group_name{1}; filename:{2}file_offset:{3}download_file_bytes_length:{4}",endPoint,group_name,filename,file_offset,download_file_bytes_length), ParsePackageView.Download, stream, Protocol.STORAGE_PROTO_CMD_RESP, -1);
                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Decrement();

                if (pkgInfo == null)
                {
                    throw new Exception(string.Format("StorageClient.Download(): resp pkg info is null;endPoint:{0};group_name:{1};filename:{2};file_offset:{3};download_file_bytes_length:{4}",endPoint,group_name,filename,file_offset,download_file_bytes_length));

                }

                if (pkgInfo.ErrorCode != 0)
                {
                    if (pkgInfo.Body == null)
                    {
                        // 对应那种返回文件块为空的情况
                        throw new Exception(string.Format("StorageClient.Download(): ErrorCode = {0}, RespPkgBody is null, while headerLength = {1}, downloadFileLength = {2}, groupFileName = {3}, FileName = {4}, Server[{5}:{6}] ", pkgInfo.ErrorCode, header.Length, download_file_bytes_length, group_name, filename, storage_server.IP, storage_server.Port));
                    }
                    else
                    {
                       throw new Exception(string.Format("StorageClient.Download(): ErrorCode = {0}, RespPkgBody not null, while headerLength = {1}, downloadFileLength = {2}, groupFileName = {3}, FileName = {4}, Server[{5}:{6}] ", pkgInfo.ErrorCode, header.Length, download_file_bytes_length, group_name, filename, storage_server.IP, storage_server.Port));
                    }

                }
                call_back(pkgInfo.Body);
                ConnectionManager.Instance.PutBackConnection(LinkType.Storages, endPoint, conn);
            }
            catch (Exception ex)
            {
                //_tracing.ErrorFmt(ex, "StorageClient.Download(): download ip:{0};port:{1};group_name:{2};filename:{3};file_offset:{4};download_file_bytes_length:{5};", storage_server.IP, storage_server.Port, group_name, filename, file_offset, download_file_bytes_length);
                if (conn != null)
                {
                    conn.Close();
                }
                call_back(null);
            }
        }
        public static void Delete(StorageServer storage_server, string group_name, string filename, Action<bool> call_back)
        {      
            FdfsConnection conn = null;

            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(storage_server.IP), storage_server.Port);
                conn = ConnectionManager.Instance.GetConnection(LinkType.Storages, endPoint);

                byte[] header;
                byte[] filename_bs;
                byte[] group_name_bs;
                byte[] sizeBytes;
                int offset;
                long body_len;
                byte cmd;

                Encoding encode = Encoding.GetEncoding("ISO8859-1");

                cmd = Protocol.STORAGE_PROTO_CMD_DELETE_FILE;
                sizeBytes = new byte[Protocol.FDFS_GROUP_NAME_MAX_LEN];

                group_name_bs = encode.GetBytes(group_name);
                Array.Copy(group_name_bs, 0, sizeBytes, 0, group_name_bs.Length);

                filename_bs = encode.GetBytes(filename);
                body_len = sizeBytes.Length + filename_bs.Length;
                offset = 0;

                header = Protocol.PackHeader(cmd, body_len, (byte)0);
                byte[] wholePkg = new byte[(int)(header.Length + body_len)];
                Array.Copy(header, 0, wholePkg, 0, header.Length);
                Array.Copy(sizeBytes, 0, wholePkg, header.Length, sizeBytes.Length);

                offset = header.Length + sizeBytes.Length;

                Array.Copy(filename_bs, 0, wholePkg, offset, filename_bs.Length);

                //if (ConfigManager.Instance.DumpObjEnable)
                //    _tracing.InfoFmt("delete storage_server:{0};group_name:{1};filename:{2};", endPoint, group_name, filename, ObjectHelper.DumpObject(header));

                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Increment();
                conn.Write(wholePkg);
                //FdfsPerfCounter.Instance.NumberOfSendPendingStorages.Decrement();

                Stream stream = conn.GetStream();

                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Increment();
                ReceivedPackage pkgInfo = Protocol.PackPackage(string.Format("groupName:{0};filename:{1}",group_name,filename), ParsePackageView.Delete, stream, Protocol.STORAGE_PROTO_CMD_RESP, -1);
                //FdfsPerfCounter.Instance.NumberOfReceivePendingStorages.Decrement();


                call_back(true);

                ConnectionManager.Instance.PutBackConnection(LinkType.Storages, endPoint, conn);
            }
            catch (Exception ex)
            {
                //_tracing.ErrorFmt(ex, "delete ip:{0};port:{1};group_name:{2};filename:{3};", storage_server.IP,storage_server.Port, group_name, filename);
                if (conn != null)
                {
                    conn.Close();
                }
                call_back(false);
            }
        }
        #endregion
    }
}
