using WT.Components.Common.Utility;
using WT.Services.Fs.Business;
using WT.Services.Fs.Business.Interfaces;
using WT.Services.Fs.Models;
using WT.Services.Fs.WebApi.Models;
using ServiceStack.ServiceInterface;
using System;
using System.IO;

namespace WT.Services.Fs.WebApi
{
    public class CommonFileServiceHost:Service
    {
        /// <summary>
        /// 文件流异步上传，返回文件guid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(AsyncsFileUploadRequest request)
        {
            int code = (int)FS_ErrorCode.Request_Success;
            if (base.Request.Files.Length == 0)
            {
                LogUtil.Info(" request.file is null");
                code = (int)FS_ErrorCode.RequestFile_Null;
                return new AsyncsFileUploadResponse { ret = code, message = ErrorCodeDic.GetInstance().CodeMessage(code), File_guid = string.Empty };
            }

            AsyncsFileUploadResponse response = new AsyncsFileUploadResponse();

            try
            {
                ICommonFilesService fileService = new CommonFilesService();
                string fileName = (string.IsNullOrEmpty(request.file_name)) ? base.Request.Files[0].FileName : request.file_name;
                byte[] bytes = streamToBytes(base.Request.Files[0].InputStream);

                code = RequestValid(request, bytes);

                if (code != (int)FS_ErrorCode.Request_Success)
                    return new AsyncsFileUploadResponse { ret = code, message = ErrorCodeDic.GetInstance().CodeMessage(code), File_guid = string.Empty };

                response = fileService.AsynFileUpload(fileName, bytes);
            }
            catch (Exception e)
            {
                string err = string.Format("CommonFileServiceHost.AsyncsFileUploadRequest error = {0}, request = {1}", e.ToString(), JsonUtil<AsyncsFileUploadRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;

        }

        /// <summary>
        /// 文件通过guid获取存储url地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(AsyncFileDownloadRequest request)
        {
            AsyncFileDownloadResponse response = new AsyncFileDownloadResponse();

            try
            {
                string guid = request.file_guid;
                ICommonFilesService filesvr = new CommonFilesService();
                response = filesvr.AsynFileDownload(guid);
            }
            catch(Exception e)
            {
                string err = string.Format("CommonFileServiceHost.AsyncFileDownloadRequest error = {0}, request = {1}", e.ToString(), JsonUtil<AsyncFileDownloadRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;
        }

        /// <summary>
        /// 文件二进制流上传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(FileUploadRequest request)
        {
            int code = (int)FS_ErrorCode.Request_Success;

            if (null == request.file_buffer || request.file_buffer.Length == 0)
            {
                LogUtil.Info("Error: request.RequestStream is null.");
                code = (int)FS_ErrorCode.RequestParam_Err;
                return new FileUploadResponse { ret = code, message = ErrorCodeDic.GetInstance().CodeMessage(code), File_guid = string.Empty };
            }
            if (string.IsNullOrEmpty(request.file_name))
            {
                LogUtil.Info("Error: request.FileName is null.");
                code = (int)FS_ErrorCode.RequestParam_Err;
                return new FileUploadResponse { ret = code, message = ErrorCodeDic.GetInstance().CodeMessage(code), File_guid = string.Empty };
            }

            FileUploadResponse response = new FileUploadResponse();

            try
            {
                ICommonFilesService fileSvr = new CommonFilesService();
                response = fileSvr.FileUpload(request.file_name, request.file_size, request.file_buffer);
            }
            catch (Exception e)
            {
                string err = string.Format("CommonFileServiceHost.AsyncFileDownloadRequest error = {0}, request = {1}", e.ToString(), JsonUtil<FileUploadRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;
        }

        /// <summary>
        /// 文件二进制流下载
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(FileDownloadRequest request)
        {
            int code = (int)FS_ErrorCode.Request_Success;

            if (string.IsNullOrEmpty(request.file_guid))
            {
                code = (int)FS_ErrorCode.RequestParam_Err;
                return new FileDownloadResponse { ret = code, message = ErrorCodeDic.GetInstance().CodeMessage(code), File_name = string.Empty, File_buffer = null };
            }

            FileDownloadResponse response = new FileDownloadResponse();
            try
            {
                ICommonFilesService fileSvr = new CommonFilesService();
                response = fileSvr.FileDownload(request.file_guid);
            }
            catch (Exception e)
            {
                string err = string.Format("CommonFileServiceHost.FileDownloadRequest error = {0}, request = {1}", e.ToString(), JsonUtil<FileDownloadRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;
             
        }

        /// <summary>
        /// 文件流转字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] streamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;

        }

        /// <summary>
        /// 文件流异步上传有效性校验
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private int RequestValid(AsyncsFileUploadRequest request, byte[] bytes)
        {
            int errorCode = (int)FS_ErrorCode.Request_Success;

            if (base.Request.Files.Length == 0)
            {
                LogUtil.Info("Error: request.file is null");
                errorCode = (int)FS_ErrorCode.RequestFile_Null;
            }
            if (request.file_size == 0 || base.Request.FormData["file_size"] != bytes.Length.ToString())
            {
                LogUtil.Info("Error: request file is not valid, please check and retry.");
                errorCode = (int)FS_ErrorCode.RequestFile_Size_Invalid;
            }

            return errorCode;
        }
    }
}