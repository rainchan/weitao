
using System;
using System.IO;

namespace WT.Components.FastDfs.FDFSClient
{
    //using Imps.Services.CommonV4;
    //using Imps.Services.CloudFile.Common;
    //using Imps.Services.CloudFile.Config;

    public enum ParsePackageView
    {
        Upload = 1,
        Download = 2,
        Delete = 3,
        Tracker = 4
    }


    public class Protocol
    {
        public const byte FDFS_PROTO_CMD_QUIT = 82;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_GROUP = 91;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_STORAGE = 92;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE = 101;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE = 102;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE = 103;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE = 104;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL = 105;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ALL = 106;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL = 107;
        public const byte TRACKER_PROTO_CMD_RESP = 100;
        public const byte FDFS_PROTO_CMD_ACTIVE_TEST = 111;
        public const byte STORAGE_PROTO_CMD_UPLOAD_FILE = 11;
        public const byte STORAGE_PROTO_CMD_DELETE_FILE = 12;
        public const byte STORAGE_PROTO_CMD_SET_METADATA = 13;
        public const byte STORAGE_PROTO_CMD_DOWNLOAD_FILE = 14;
        public const byte STORAGE_PROTO_CMD_GET_METADATA = 15;
        public const byte STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE = 21;
        public const byte STORAGE_PROTO_CMD_RESP = TRACKER_PROTO_CMD_RESP;
        public const byte STORAGE_SET_METADATA_FLAG_OVERWRITE = (byte)'O';
        public const byte STORAGE_SET_METADATA_FLAG_MERGE = (byte)'M';
        public const byte TRACKER_PROTO_PKG_LEN_SIZE = 8;
        public const byte TRACKER_PROTO_CMD_SIZE = 1;
        public const byte FDFS_GROUP_NAME_MAX_LEN = 16;
        public const byte FDFS_IPADDR_SIZE = 16;
        public const string FDFS_RECORD_SEPERATOR = "\u0001";
        public const string FDFS_FIELD_SEPERATOR = "\u0002";
        public const int TRACKER_QUERY_STORAGE_FETCH_BODY_LEN = FDFS_GROUP_NAME_MAX_LEN + FDFS_IPADDR_SIZE - 1 + TRACKER_PROTO_PKG_LEN_SIZE;
        public const int TRACKER_QUERY_STORAGE_STORE_BODY_LEN = FDFS_GROUP_NAME_MAX_LEN + FDFS_IPADDR_SIZE + TRACKER_PROTO_PKG_LEN_SIZE;
        protected const int PROTO_HEADER_CMD_INDEX = TRACKER_PROTO_PKG_LEN_SIZE;
        protected const int PROTO_HEADER_STATUS_INDEX = TRACKER_PROTO_PKG_LEN_SIZE + 1;
        public const byte FDFS_FILE_EXT_NAME_MAX_LEN = 6;
        public const byte FDFS_FILE_PREFIX_MAX_LEN = 16;
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(Protocol));

        public static byte[] PackHeader(byte cmd, long packageLen, byte errno)
        {
            byte[] header = new byte[TRACKER_PROTO_PKG_LEN_SIZE + 2];
            byte[] hex_len;

            for (int i = 0; i < header.Length; i++)
            {
                header[i] = (byte)0;
            }

            hex_len = ConvertLongToBuff(packageLen);
            Array.Copy(hex_len, 0, header, 0, hex_len.Length);
            header[PROTO_HEADER_CMD_INDEX] = cmd;
            header[PROTO_HEADER_STATUS_INDEX] = errno;

            return header;
        }

        public static ReceivedHeader PackHeader(string blockKey,ParsePackageView view,Stream stream, byte cmd, long bodyLen)
        {
            byte[] header = new byte[TRACKER_PROTO_PKG_LEN_SIZE + 2];
            int bytes;
            long pkg_len;

            if ((bytes = stream.Read(header, 0, header.Length)) != header.Length)
            {
                throw new IOException("receive package size " + bytes + " != " + header.Length);
            }

            if (header[PROTO_HEADER_CMD_INDEX] != cmd)
            {
                throw new IOException("receive cmd: " + header[PROTO_HEADER_CMD_INDEX] + " is not correct, expect cmd: " + cmd);
            }

            if (header[PROTO_HEADER_STATUS_INDEX] != 0)
            {
                return new ReceivedHeader(header[PROTO_HEADER_STATUS_INDEX], 0);
            }

            pkg_len = ConvertBuffToLong(header, 0);

            //if(ConfigManager.Instance.DumpObjEnable)
            //_tracing.InfoFmt("cmd：{0}；bodyLen：{1};pkg_len:{2};blockKey:{3};view:{4};datat:{5}", cmd, bodyLen, pkg_len, blockKey, view,ObjectHelper.DumpObject(header));
                
            if (pkg_len < 0)
            {
                throw new IOException("receive body length: " + pkg_len + " < 0!");
            }

            if (bodyLen >= 0 && pkg_len != bodyLen)
            {
                throw new IOException("receive body length: " + pkg_len + " is not correct, expect length: " + bodyLen);
            }

            return new ReceivedHeader((byte)0, pkg_len);
        }

        public static ReceivedPackage PackPackage(string fileBlockKey,ParsePackageView view,Stream stream, byte cmd, long bodyLen)
        {
            ReceivedHeader header = PackHeader(fileBlockKey,view, stream, cmd, bodyLen);

            if (header.ErrorCode != 0)
            {
                return new ReceivedPackage(header.ErrorCode, null);
            }

            byte[] body = new byte[(int)header.BodyLength];
            int totalBytes = 0;
            int remainBytes = (int)header.BodyLength;
            int bytes;

            while (totalBytes < header.BodyLength)
            {
                if ((bytes = stream.Read(body, totalBytes, remainBytes)) <= 0)
                {
                    break;
                }

                totalBytes += bytes;
                remainBytes -= bytes;
            }

            if (totalBytes != header.BodyLength)
            {
                throw new IOException("receive package size " + totalBytes + " != " + header.BodyLength);
            }

            return new ReceivedPackage((byte)0, body);
        }

        public static byte[] ConvertLongToBuff(long n)
        {
            byte[] bs = new byte[8];

            bs[0] = (byte)((n >> 56) & 0xFF);
            bs[1] = (byte)((n >> 48) & 0xFF);
            bs[2] = (byte)((n >> 40) & 0xFF);
            bs[3] = (byte)((n >> 32) & 0xFF);
            bs[4] = (byte)((n >> 24) & 0xFF);
            bs[5] = (byte)((n >> 16) & 0xFF);
            bs[6] = (byte)((n >> 8) & 0xFF);
            bs[7] = (byte)(n & 0xFF);

            return bs;
        }

        public static long ConvertBuffToLong(byte[] bs, int offset)
        {
            return (((long)(bs[offset] >= 0 ? bs[offset] : 256 + bs[offset])) << 56) |
                    (((long)(bs[offset + 1] >= 0 ? bs[offset + 1] : 256 + bs[offset + 1])) << 48) |
                    (((long)(bs[offset + 2] >= 0 ? bs[offset + 2] : 256 + bs[offset + 2])) << 40) |
                    (((long)(bs[offset + 3] >= 0 ? bs[offset + 3] : 256 + bs[offset + 3])) << 32) |
                    (((long)(bs[offset + 4] >= 0 ? bs[offset + 4] : 256 + bs[offset + 4])) << 24) |
                    (((long)(bs[offset + 5] >= 0 ? bs[offset + 5] : 256 + bs[offset + 5])) << 16) |
                    (((long)(bs[offset + 6] >= 0 ? bs[offset + 6] : 256 + bs[offset + 6])) << 8) |
                     ((long)(bs[offset + 7] >= 0 ? bs[offset + 7] : 256 + bs[offset + 7]));
        }

        private static ReceivedHeader ProcessHeader(byte[] header, byte cmd, long bodyLen)
        {
            long pkg_len;
            if (header[PROTO_HEADER_CMD_INDEX] != cmd)
            {
                throw new IOException("receive cmd: " + header[PROTO_HEADER_CMD_INDEX] + " is not correct, expect cmd: " + cmd);
            }

            if (header[PROTO_HEADER_STATUS_INDEX] != 0)
            {
                return new ReceivedHeader(header[PROTO_HEADER_STATUS_INDEX], 0);
            }

            pkg_len = ConvertBuffToLong(header, 0);

            if (pkg_len < 0)
            {
                throw new IOException("receive body length: " + pkg_len + " < 0!");
            }

            if (bodyLen >= 0 && pkg_len != bodyLen)
            {
                throw new IOException("receive body length: " + pkg_len + " is not correct, expect length: " + bodyLen);
            }

            return new ReceivedHeader((byte)0, pkg_len);
        }
    }
}
