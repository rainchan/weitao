//using Imps.Services.CloudFile.Config;

namespace WT.Components.FastDfs.FDFSClient
{
    public class FdfsHelper
    {
        public static byte[] GetSocketKeepAlive()
        {
            byte[] KeepAlive = new byte[12];
            //if (ConfigManager.Instance.SocketKeepAliveEnable)
            if(FdfsCommon.SocketKeepAliveEnable)
            {
                KeepAlive[0] = (byte)0x01;
                //int value = ConfigManager.Instance.SocketKeepAliveTimeout;
                int value = FdfsCommon.SocketKeepAliveTimeout;
                for (int i = 4; value != 0 && i < 8; ++i)
                {
                    KeepAlive[i] = (byte)(value & 0xFF);
                    value >>= 8;
                }
                //value = ConfigManager.Instance.SocketKeepAliveInterval;
                value = FdfsCommon.SocketKeepAliveInterval;
                for (int i = 8; value != 0 && i < 12; ++i)
                {
                    KeepAlive[i] = (byte)(value & 0xFF);
                    value >>= 8;
                }
            }
            return KeepAlive;
        }
    }
}
