using System;
using WT.Components.FastDfs.Entities;
using WT.Components.FastDfs.FDFSClient;

namespace WT.Components.FastDfs
{
    public class FSUploadTransaction
    {
        private FSUploadEntity uploadEntity;
        private Action<FSUploadResultEntity, Exception> UploadCallBack { get; set; }
        public string FilePath { get; set; }
        public Exception InnerEx { get; set; }

        public FSUploadTransaction(FSUploadEntity entity)
        {
            uploadEntity = entity;
        }

        public void BeginUpload(Action<FSUploadResultEntity, Exception> uploadCallback, int timeout)
        {
            UploadCallBack = uploadCallback;

            Action<FSUploadEntity> asyncUploadAction = new Action<FSUploadEntity>((fsUpload) =>
            {
                StorageServer storageServer = TrackerClient.GetStorageServer();
                if (storageServer == null)
                {
                    InnerEx = new Exception("FSUploadTransaction.BeginTTSSynth(): Failed to get Storage server address: Please check ths fdfs storage's config and the fdfs tracker is running.");
                    return;
                }
                
                Exception outputCallbackEx = null;
                StorageClient.Upload(storageServer, uploadEntity.FileExtName, uploadEntity.FileSize, uploadEntity.FileBuffer, string.Empty, (filePathArray) =>
                {
                    try
                    {
                        FilePath = string.Format("{0}/{1}", filePathArray[0], filePathArray[1]);   // 0: groupName, 1:fileName
                    }
                    catch (Exception ex2)
                    {
                        string err = string.Format("FSUploadTransaction.BeginUpload: Exception when executing OutputCallback of StorageClient.Upload(), FileName={0}, storageServer={1}", uploadEntity.FileName, storageServer.IP);
                        outputCallbackEx = new Exception(err, ex2);
                    }
                });                

                if(null != outputCallbackEx)
                {
                    InnerEx = outputCallbackEx;
                }
            });
            
            asyncUploadAction.BeginInvoke(uploadEntity, new AsyncCallback(CallbackForAsyncAction), this);               
        }

        private static void CallbackForAsyncAction(IAsyncResult asyncResult)
        {
            FSUploadTransaction trans = (FSUploadTransaction)asyncResult.AsyncState;
            FSUploadResultEntity resultEntity = null;
            if(null == trans.InnerEx)
            {
                resultEntity = new FSUploadResultEntity();
                resultEntity.FilePathOnFDFS = trans.FilePath;
            }
            trans.UploadCallBack(resultEntity, trans.InnerEx);
        }
    }
}
