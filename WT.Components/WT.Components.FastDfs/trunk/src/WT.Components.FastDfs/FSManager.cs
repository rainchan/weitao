using System;
using System.Collections;
using WT.Components.FastDfs.Entities;
using WT.Components.FastDfs.FDFSClient;

namespace WT.Components.FastDfs
{
    public class FSManager
    {
        private static bool bInit = false;

        public static void Initialize(Hashtable trackerTable, Hashtable storagerTable)
        {
            if (bInit)
                return;
            if (null == trackerTable || trackerTable.Values.Count == 0 || null == storagerTable || storagerTable.Values.Count == 0)
                throw new Exception("FSManager.Initialize(): FSLibrary initialization failed: storageServerList is empty. Please input at least 1 storage server address.");

            ConnectionManager.Instance.Initialize(trackerTable, storagerTable);
            bInit = true;
        }

        // 同步上传接口
        public static void UploadFile(byte[] fileBuffer, string fileExtName, Action<Exception, string> fileUploadCallback)
        {
            if (!bInit)
            {
                Exception ex1 = new Exception("FSManager.UploadFile(): FSLibrary not initialized: please invoke FSManager.Initialize() first.");
                fileUploadCallback(ex1, string.Empty);
                return;
            }
            if (null == fileBuffer)
            {
                Exception ex2 = new Exception("FSManager.UploadFile(): failed to upload file : file buffer is null.");
                fileUploadCallback(ex2, string.Empty);
                return;
            }

            StorageServer storageServer = TrackerClient.GetStorageServer();
            if (storageServer == null)
            {
                throw new Exception("FSManager.UploadFile(): Failed to get Storage server address: Please check ths fdfs storage's config and the fdfs tracker is running.");
            }

            string filePath = string.Empty;
            StorageClient.Upload(storageServer, fileExtName, fileBuffer.Length, fileBuffer, string.Empty, (filePathArray) =>
            {
                filePath = string.Format("{0}/{1}", filePathArray[0], filePathArray[1]);   // 0: groupName, 1:fileName
            });

            fileUploadCallback(null, filePath);
        }

        // 异步上传接口
        public static void BeginUploadFile(byte[] fileBuffer, string fileName, string fileExtName, long fileSize, Action<FSUploadResultEntity, Exception> fileUploadCallback)
        {
            if (!bInit)
            {
                Exception ex1 = new Exception("FSManager.BeginUploadFile(): FSLibrary not initialized: please invoke FSManager.Initialize() first.");
                fileUploadCallback(null, ex1);
                return;
            }
            if (null == fileBuffer)
            {
                Exception ex2 = new Exception("FSManager.BeginUploadFile(): failed to upload file : file buffer is null.");
                fileUploadCallback(null, ex2);
                return;
            }

            FSUploadEntity entity = new FSUploadEntity();
            entity.FileBuffer = fileBuffer;
            entity.FileName = fileName;
            entity.FileExtName = fileExtName;
            entity.FileSize = fileSize;
            FSUploadTransaction uploadTrans = new FSUploadTransaction(entity);
            uploadTrans.BeginUpload(fileUploadCallback, 30000);
        }

        // filePath format: gourpName/FileName
        public static void DownloadFile(string filePath, Action<Exception, byte[]> fileDownloadCallback)
        {
            if (!bInit)
            {
                Exception ex1 = new Exception("FSManager.DownloadFile(): FSLibrary not initialized: please invoke FSManager.Initialize() first.");
                fileDownloadCallback(ex1, null);
                return;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                Exception ex2 = new Exception("FSManager.DownloadFile(): failed to upload file : file buffer is null.");
                fileDownloadCallback(ex2, null);
                return;
            }

            string[] filePathArray = new string[] { string.Empty, string.Empty };
            int firstSlashIndex = filePath.IndexOf('/');
            filePathArray[0] = filePath.Substring(0, firstSlashIndex);
            filePathArray[1] = filePath.Substring(firstSlashIndex + 1);
            StorageServer storageServer = TrackerClient.GetStorageServer(filePathArray[0], filePathArray[1]);
            if (storageServer == null)
            {
                throw new Exception("FSManager.UploadFile(): Failed to get Storage server address: Please check ths fdfs storage's config and the fdfs tracker is running.");
            }

            byte[] buffer = null;
            StorageClient.Download(storageServer, filePathArray[0], filePathArray[1], 0, 0, (fileBuffer) =>
            {
                buffer = fileBuffer;
            });

            fileDownloadCallback(null, buffer);
        }
    }
}
