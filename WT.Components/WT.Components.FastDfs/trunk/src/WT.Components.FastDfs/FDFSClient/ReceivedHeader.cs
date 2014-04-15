
namespace WT.Components.FastDfs.FDFSClient
{
    public class ReceivedHeader
    {
        public byte ErrorCode { get; set; }

        public long BodyLength { get; set; }

        public ReceivedHeader(byte errorCode, long bodyLength)
        {
            ErrorCode = errorCode;
            BodyLength = bodyLength;
        }
    }
}
