using System.Collections.Generic;
using System.Net;
using System.Collections;
using WT.Components.Common.Utility;

//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Config;

//using Imps.Services.CloudFile.Common;

namespace WT.Components.FastDfs.FDFSClient
{
    public enum LinkType
    {
        Tracker = 0,
        Storages = 1
    }

    public class ConnectionManager
    {
        //private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(ConnectionManager));
        private static ConnectionManager instance = new ConnectionManager();
        private Dictionary<IPEndPoint, ConnectionGroup> _groupsTracker;
        private Dictionary<IPEndPoint, ConnectionGroup> _groupsStorages;
        private object _syncRoot = new object();
        private bool _inited = false;
        private int _trackerSeq = 0;
        private int _storagesSeq = 0;
        private int _trackerNum = 0;
        private int _storagesNum = 0;
        private object _syncTracker = new object();
        private object _syncStorages = new object();

        public ConnectionManager()
        {    
       
        }

        // groupTracks: [defaultgroup: "121.199.21.50:22122,121.199.21.53:22122"]
        public void Initialize(Hashtable groupTracks, Hashtable groupStroages)
        {
            if (!_inited)
            {
                LogUtil.Info(string.Format("ConnectionManager.Initialize: start"));
                _groupsTracker = new Dictionary<IPEndPoint, ConnectionGroup>();
                _groupsStorages = new Dictionary<IPEndPoint, ConnectionGroup>();

                string[] storages = groupStroages["DefaultGroup"].ToString().Split(new char[] { ',' });

                foreach (string s in storages)
                {
                    string[] ss = s.Split(':');
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ss[0]), int.Parse(ss[1]));
                    ConnectionGroup group = new ConnectionGroup(this,endPoint,LinkType.Storages);
                    this._groupsStorages.Add(endPoint, group);
                }
                _storagesNum = storages.Length;


                string[] tracker = groupTracks["DefaultGroup"].ToString().Split(new char[] { ',' });
                foreach (string s in tracker)
                {
                    string[] ss = s.Split(':');
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ss[0]), int.Parse(ss[1]));
                    ConnectionGroup group = new ConnectionGroup(this, endPoint,LinkType.Tracker);
                    this._groupsTracker.Add(endPoint, group);
                }
                _trackerNum = tracker.Length;
                _inited = true;

                //FdfsPerfCounter.Instance.NumberOfConnectionGroupTracker.SetRawValue(_trackerNum);
                //FdfsPerfCounter.Instance.NumberOfConnectionGroupStorages.SetRawValue(_storagesNum);
            }

            LogUtil.Info(string.Format("ConnectionManager.Initialize has init"));
        }

        public static ConnectionManager Instance
        {
            get { return instance; }
        }

        public FdfsConnection GetConnection(LinkType link)
        {

            if (link == LinkType.Tracker)
            {
                int i = 0;
                IPEndPoint endPoint = null;
                int trackerSeq = 0;
                lock (_syncTracker)
                {
                    _trackerSeq++;
                    if (_trackerSeq >= _trackerNum)
                        _trackerSeq = 0;
                    trackerSeq = _trackerSeq;
                }

                foreach (IPEndPoint ip in _groupsTracker.Keys)
                {
                    if (i == trackerSeq)
                    {
                        endPoint = ip;
                        break;
                    }
                        i++;
                }

                return GetConnection(link, endPoint);
            }
            else
            {
                int i = 0;
                IPEndPoint endPoint = null;
                int storagesSeq = 0;
                lock (_syncStorages)
                {
                    _storagesSeq++;
                    if (_storagesSeq >= _storagesNum)
                        _storagesSeq = 0;
                    storagesSeq = _storagesSeq;
                }


                foreach (IPEndPoint ip in _groupsStorages.Keys)
                {
                    if (i == storagesSeq)
                    {
                        endPoint = ip;
                        break;
                    }
                    i++;
                }

                return GetConnection(link, endPoint);
            }

        }


        public FdfsConnection GetConnection(LinkType link,IPEndPoint remoteEP)
        {
            Dictionary<IPEndPoint, ConnectionGroup> groups = GetGroup(link);
            FdfsConnection conn = null;

            lock (_syncRoot)
            {
                ConnectionGroup group = null;
                if (groups.TryGetValue(remoteEP, out group))
                {
                    conn = group.GetOne();
                }
                else
                {
                    group = new ConnectionGroup(this, remoteEP,link);
                    conn = group.GetOne();
                    groups.Add(remoteEP, group);
                }
            }
            LogUtil.Info(string.Format("FdfsConnection GetConnection(LinkType {0},IPEndPoint {1}) port:{2} connect {3}",link.ToString(), remoteEP.Address, remoteEP.Port, conn.IsConnected.ToString()));
            return conn;
        }

        public void PutBackConnection(LinkType link, IPEndPoint remoteEP, FdfsConnection conn)
        {
            Dictionary<IPEndPoint, ConnectionGroup> groups = GetGroup(link);
            lock (_syncRoot)
            {
                ConnectionGroup group = null;
                if (groups.TryGetValue(remoteEP, out group))
                {
                    group.PutBackConnection(conn);
                } 
            }
        }

        private Dictionary<IPEndPoint, ConnectionGroup> GetGroup(LinkType link)
        {
            Dictionary<IPEndPoint, ConnectionGroup> groups = null;
            if (link == LinkType.Storages)
                groups = _groupsStorages;
            else
                groups = _groupsTracker;

            return groups;
        }
    }
}
