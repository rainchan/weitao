
namespace WT.Components.FastDfs.FDFSClient
{
    public class ReceivedPackage
    {
        public byte ErrorCode { get; set; }

        public byte[] Body { get; set; }

        public ReceivedPackage(byte errorCode, byte[] body)
        {
            ErrorCode = errorCode;
            Body = body;
        }
    }
}
