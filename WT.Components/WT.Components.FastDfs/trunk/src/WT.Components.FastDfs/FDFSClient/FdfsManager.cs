//using Imps.Services.CommonV4;
//using Imps.Services.CloudFile.Config;
//using Imps.Services.CloudFile.Common;
//using Imps.Services.CloudFile.External;

namespace WT.Components.FastDfs.FDFSClient
{
    public class FdfsManager
    {
        /*
        private static ITracing _tracing = TracingManager.GetTracing(typeof(FdfsManager));
        private static FdfsManager _instance = new FdfsManager();
        private int _count;
        private int _capacity; 
        public static FdfsManager Instance
        {
            get { return _instance; }
        }
        private FdfsManager()
        {
        }

        private ParallelQueue<int, RpcServerContext> _uploadOfflineFileQueue;
        private ParallelQueue<int, RpcServerContext> _downloadOfflineFileQueue;
        private ParallelQueue<int, RpcServerContext> _deleteOfflineFileQueue;
        public void Initialize()
        {
            _capacity = ConfigManager.Instance.ParallelQueueCount;

            _uploadOfflineFileQueue = new ParallelQueue<int, RpcServerContext>(
              "UploadOfflineFile",
              ConfigManager.Instance.QueueMaxBatchCount,
              ConfigManager.Instance.QueueMaxIdleMs,
              UploadOffileFileAction);

            _downloadOfflineFileQueue = new ParallelQueue<int, RpcServerContext>(
             "DownloadOfflineFile",
             ConfigManager.Instance.QueueMaxBatchCount,
             ConfigManager.Instance.QueueMaxIdleMs,
             DownloadOffileFileAction);

            _deleteOfflineFileQueue = new ParallelQueue<int, RpcServerContext>(
            "DeleteOfflineFile",
            ConfigManager.Instance.QueueMaxBatchCount,
            ConfigManager.Instance.QueueMaxIdleMs,
            DeleteOffileFileAction);
        }

        //不用很精准，就是分散
        private int GetKey()
        {
            _count++;
            if (_count > _capacity)
            {
                _count = 0;
            }
            return _count;
        }

        public void EnqueueUploadOffileFile(RpcServerContext context)
        {
            _uploadOfflineFileQueue.Enqueue(GetKey(), context);
        }

        public void EnqueueDownloadOffileFile(RpcServerContext context)
        {
            _downloadOfflineFileQueue.Enqueue(GetKey(), context);
        }

        public void EnqueueDeleteOffileFile(RpcServerContext context)
        {
            _deleteOfflineFileQueue.Enqueue(GetKey(), context);
        }

        private void UploadOffileFileAction(int key, RpcServerContext[] contextList)
        {
            foreach (RpcServerContext context in contextList)
            {
                try
                {

                    StoreDataArgs args = context.GetArgs<StoreDataArgs>();
                    StorageServer server = TrackerClient.GetStorageServer();
                    if (server == null)
                    {
                        context.ReturnError(RpcErrorCode.Unknown, new Exception("StoreData无法获取到StorageServer"));
                        return;
                    }

                    if (null == args.Buffer || args.Buffer.Length == 0)
                    {
                        _tracing.ErrorFmt("Before StorageClient.Upload() invoked: FileSize={0}, RealBufferSize=0", args.FileSize);
                    }

                    StorageClient.Upload(server, null, args.FileSize, args.Buffer,args.FileBlockKey,
                        (ret) =>
                        {
                            string fileId = null;
                            cf_err_code errCode = cf_err_code.cf_err_code_unknown;
                            StoreDataResult result = new StoreDataResult();
                            if (ret != null)
                            {
                                fileId = ret[0] + @"//" + ret[1];
                                _tracing.InfoFmt("StorageClient.Upload() invoked: server={0},fileSize={1},physicalPath={2}", server, args.FileSize, fileId);
                                errCode = cf_err_code.cf_err_code_ok;
                            }
                            else
                            {
                                _tracing.ErrorFmt("StorageClient.Upload() invoked: server={0},fileSize={1},physicalPath=null", server, args.FileSize);
                                fileId = null;
                                errCode = cf_err_code.cf_err_code_server_error;
                            }
                            result.FileId = fileId;
                            result.ResultCode = errCode;
                            context.Return<StoreDataResult>(result);
                        });
                }
                catch (Exception ex)
                {
                    context.ReturnError(RpcErrorCode.Unknown, ex);
                    _tracing.Error(ex, "FDFS StoreData Error");
                }
            }
        }

        private void DownloadOffileFileAction(int key, RpcServerContext[] contextList)
        {
            foreach (RpcServerContext context in contextList)
            {
                try
                {
                    FetchDataArgs args = context.GetArgs<FetchDataArgs>();
                    string[] fileId = args.FileId.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                    StorageServer server = TrackerClient.GetStorageServer(fileId[0], fileId[1]);
                    if (server == null)
                    {
                        context.ReturnError(RpcErrorCode.Unknown, new Exception("FetchData无法获取到StorageServer"));
                        return;
                    }
                    StorageClient.Download(server, fileId[0], fileId[1], args.Offset, args.FetchBytes,
                        (ret) =>
                        {
                            try
                            {
                                FetchDataResult result = new FetchDataResult();
                                result.Buffer = ret;
                                result.ResultCode = cf_err_code.cf_err_code_ok;
                                context.Return<FetchDataResult>(result);
                            }
                            catch (Exception ex)
                            {
                                context.ReturnError(RpcErrorCode.Unknown, ex);
                                _tracing.ErrorFmt(ex, "FetchData [Inner delegate]Error");
                            }
                        });
                }
                catch (Exception ex)
                {
                    context.ReturnError(RpcErrorCode.Unknown, ex);
                    _tracing.Error(ex, "FDFS FetchData Error");
                }
            }
        }

        private void DeleteOffileFileAction(int key, RpcServerContext[] contextList)
        {
            foreach (RpcServerContext context in contextList)
            {
                try
                {
                    DeleteDataArgs args = context.GetArgs<DeleteDataArgs>();
                    if (string.IsNullOrEmpty(args.FileId))
                        return;
                    string[] fileId = args.FileId.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                    StorageServer server = TrackerClient.GetStorageServer(fileId[0], fileId[1]);
                    if (server == null)
                    {
                        context.ReturnError(RpcErrorCode.Unknown, new Exception("DeleteData无法获取到StorageServer"));
                        return;
                    }

                    StorageClient.Delete(server, fileId[0], fileId[1],
                        (ret) =>
                        {
                            DeleteDataResult result = new DeleteDataResult();
                            result.ResultCode = ret ? cf_err_code.cf_err_code_ok : cf_err_code.cf_err_code_server_error;
                            context.Return<DeleteDataResult>(result);

                        });
                }
                catch (Exception ex)
                {
                    context.ReturnError(RpcErrorCode.Unknown, ex);
                    _tracing.Error(ex, "FDFS DeleteData Error");
                }
            }
        }

        private void EnqueueLog(WriteLogArgs logArgs)
        {
            try
            {
                LOGHelper.Enqueue(logArgs);
            }
            catch (Exception ex)
            {
                _tracing.ErrorFmt(ex, null);
            }
        }
        */
    }
    
}
