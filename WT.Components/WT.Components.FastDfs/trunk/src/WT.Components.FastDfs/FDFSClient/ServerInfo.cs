
namespace WT.Components.FastDfs.FDFSClient
{
    public class ServerInfo
    {
        public string IP { get; set; }

        public int Port { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", IP, Port);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
