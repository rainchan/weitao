using System.Net.Sockets;
using System.Net;
using System.IO;
//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Config;

//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    public class FdfsConnection
    {
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(FdfsConnection));
        private Socket _socket = null;
        private Stream _stream;
        private ConnectionGroup _group;
        private static readonly byte[] KeepAlive = FdfsHelper.GetSocketKeepAlive();
        private IPEndPoint _localEP;
        private IPEndPoint _remoteEP;
        //private int _recvBufferSize = ConfigManager.Instance.ReceiveBufferSize;
        //private int _sendBufferSize = ConfigManager.Instance.SendBufferSize;
        private int _recvBufferSize = FdfsCommon.ReceiveBufferSize;
        private int _sendBufferSize = FdfsCommon.SendBufferSize;
        private object _syncClose = new object();

        private void InitSocket(Socket socket)
        {
            socket.IOControl(IOControlCode.KeepAliveValues, KeepAlive, null);
            socket.ReceiveBufferSize = _recvBufferSize;
            socket.SendBufferSize = _sendBufferSize;
            //socket.SendTimeout = ConfigManager.Instance.SendTimeout;
            //socket.ReceiveTimeout = ConfigManager.Instance.ReceiveTimeout;
            socket.SendTimeout = FdfsCommon.SendTimeout;
            socket.ReceiveTimeout = FdfsCommon.ReceiveTimeout;

            if (_sendBufferSize == 0)
                socket.NoDelay = true;

            _socket = socket;
            _localEP = _socket.LocalEndPoint as IPEndPoint;
            _remoteEP = _socket.RemoteEndPoint as IPEndPoint;

            //_tracing.InfoFmt("connection created on TCP:L{0}-R{1}", _localEP, _remoteEP);
        }

        public FdfsConnection(ConnectionGroup group,Socket socket)
        {   
            _group = group;
            InitSocket(socket);
            _stream = new BufferedStream(new NetworkStream(_socket, false));
        }

        public bool IsConnected
        {
            get { return _socket != null && _socket.Connected; }
        }

        public void Write(byte[] bytes)
        {
            int length = bytes.Length;
            //int maxSize = ConfigManager.Instance.PacketMaxSize;
            int maxSize = FdfsCommon.PacketMaxSize;
            if (length > maxSize)
            {
                int count = length / maxSize;
                int leftSize = length % maxSize;

                for (int i = 0; i < count; i++)
                {
                    _stream.Write(bytes, i * maxSize, maxSize);
                    _stream.Flush();
                }

                if (leftSize > 0)
                {
                    _stream.Write(bytes, count * maxSize, leftSize);
                    _stream.Flush();
                }
            }
            else
            {
                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
            }
        }


        public Stream GetStream()
        {
            return _stream;
        }

        public IPEndPoint RemoteEp
        {
            get { return _remoteEP; }
        }

        public void Close()
        {
            Socket s = _socket;
            if (s == null)
                return;

            lock (_syncClose)
            {
                if (_socket == null)
                    return;
                _socket = null;
            }

            //_tracing.InfoFmt("connection closed on TCP:L{0}-R{1}", _localEP, _remoteEP);
            SocketPool.Close(s,true);

            //if (this._group.Link == LinkType.Tracker)
            //    FdfsPerfCounter.Instance.NumberOfConnectionTracker.Decrement();
            //else
            //    FdfsPerfCounter.Instance.NumberOfConnectionStorages.Decrement();
        }
    }
}
