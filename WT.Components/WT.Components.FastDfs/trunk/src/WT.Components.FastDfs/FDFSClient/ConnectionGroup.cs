using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Config;
//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    public class ConnectionGroup
    {
        //private static ITracing _tracing = TracingManager.GetTracing(typeof(ConnectionGroup));
        private Queue<FdfsConnection> _connections;
        private IPEndPoint _remoteEP;
        private ConnectionManager _manager;
        private LinkType _link;
        private object _syncRoot = new object();
        public IPEndPoint RemoteEP
        {
            get { return _remoteEP; }
        }

        public LinkType Link
        {
            get { return _link; }
        }

        public ConnectionGroup(ConnectionManager manager, IPEndPoint remoteEP,LinkType link)
        {
            _link = link;
            _manager = manager;
            _remoteEP = remoteEP;
            _connections = new Queue<FdfsConnection>();
        }

        public void PutBackConnection(FdfsConnection connection)
        {
            lock (_syncRoot)
            {
                _connections.Enqueue(connection);

            }

            //if (this._link == LinkType.Tracker)
            //    FdfsPerfCounter.Instance.NumberOfConnectionQueueTracker.Increment();
            //else
            //    FdfsPerfCounter.Instance.NumberOfConnectionQueueStorages.Increment();
        }

        public FdfsConnection GetOne( )
        {
            FdfsConnection conn = null;
            lock (_syncRoot)
            {
                if (_connections.Count > 0)
                {
                    conn = _connections.Dequeue();

                    //if (this._link == LinkType.Tracker)
                    //    FdfsPerfCounter.Instance.NumberOfConnectionQueueTracker.Decrement();
                    //else
                    //    FdfsPerfCounter.Instance.NumberOfConnectionQueueStorages.Decrement();

                    return conn;
                }
            }

            Socket socket = SocketPool.GetOne();

            try
            {
                IAsyncResult result = socket.BeginConnect(_remoteEP, null, null);
                //bool success = result.AsyncWaitHandle.WaitOne(ConfigManager.Instance.ConnectTimeout, false);
                bool success = result.AsyncWaitHandle.WaitOne(FdfsCommon.ConnectTimeout, false);
                if (!success)
                {
                    //_tracing.ErrorFmt("ConnectionGroup::MakeOne:no success:_remoteEP:{0}", _remoteEP);
                    throw new SocketException();
                }

                socket.EndConnect(result);

                if (!socket.Connected)
                {
                    //_tracing.ErrorFmt("ConnectionGroup::MakeOne:socket.Connected:_remoteEP:{0}", _remoteEP);

                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                SocketPool.Close(socket, true);
                //_tracing.ErrorFmt(ex, "ConnectionGroup::MakeOne:_remoteEP:{0}", _remoteEP);
                throw ex;
            }

            //if (this._link == LinkType.Tracker)
            //    FdfsPerfCounter.Instance.NumberOfConnectionTracker.Increment();
            //else
            //    FdfsPerfCounter.Instance.NumberOfConnectionStorages.Increment();

            return new FdfsConnection(this,socket);
        }

        public void MakeOne()
        {
            Socket socket = SocketPool.GetOne();

            try
            {
                IAsyncResult result = socket.BeginConnect(_remoteEP, null, null);
                //bool success = result.AsyncWaitHandle.WaitOne(ConfigManager.Instance.ConnectTimeout, false);
                bool success = result.AsyncWaitHandle.WaitOne(FdfsCommon.ConnectTimeout, false);
                if (!success)
                {
                    //_tracing.ErrorFmt("ConnectionGroup::MakeOne:no success:_remoteEP:{0}", _remoteEP);
                    throw new SocketException();
                }

                socket.EndConnect(result);

                if (!socket.Connected)
                {
                    //_tracing.ErrorFmt("ConnectionGroup::MakeOne:socket.Connected:_remoteEP:{0}", _remoteEP);

                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                SocketPool.Close(socket, true);
                //_tracing.ErrorFmt(ex,"ConnectionGroup::MakeOne:_remoteEP:{0}", _remoteEP);
                throw ex;
            }

            lock (_syncRoot)
            {
                _connections.Enqueue(new FdfsConnection(this,socket));
            }

            //if (this._link == LinkType.Tracker)
            //    FdfsPerfCounter.Instance.NumberOfConnectionTracker.Increment();
            //else
            //    FdfsPerfCounter.Instance.NumberOfConnectionStorages.Increment();
        }
    }
}
