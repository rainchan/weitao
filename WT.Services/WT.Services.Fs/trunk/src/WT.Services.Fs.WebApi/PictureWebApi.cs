using WT.Components.Common.Utility;
using WT.Services.Fs.Business;
using WT.Services.Fs.Business.Interfaces;
using WT.Services.Fs.Models;
using WT.Services.Fs.WebApi.Models;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;

namespace WT.Services.Fs.WebApi
{
    public class PictureServiceHost:Service
    {
        /// <summary>
        /// 获取单个指定尺寸图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(FSScalePicFileRequest request)
        {
            int errCode = ImageFileRequestValid(request);
           
            if (errCode != (int)FS_ErrorCode.Request_Success)
                return new AsyncFileDownloadResponse
                    {
                        ret = errCode,
                        message = ErrorCodeDic.GetInstance().CodeMessage(errCode),
                        File_url = null
                    };

            AsyncFileDownloadResponse response = new AsyncFileDownloadResponse();

            try
            {
                IPictureFileService picSvr = new PictureFileService();
                response = picSvr.ScalePictureFile(request.file_guid, request.width, request.height);
            }
            catch(Exception e)
            {
                string err = string.Format("PictureServiceHost.FSScalePicFileRequest error = {0}, request = {1}", e.ToString(), JsonUtil<FSScalePicFileRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;
        }

        /// <summary>
        /// 同步获取多张比列缩放图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Post(FSScalePicFileListRequest request)
        {
            int errCode = ImageFilesValid(request);

            if (errCode != (int)FS_ErrorCode.Request_Success)
            {
                return new ScalePicFileListModel { 
                    ret = errCode,
                    message = ErrorCodeDic.GetInstance().CodeMessage(errCode),
                    picUrls = null
                };
            }

            var response = new ScalePicFileListModel();
            try
            {
                IPictureFileService picSvr = new PictureFileService();
                response = picSvr.ScalePictureFile(request.file_guid, JsonUtil<List<string>>.FromJosn(request.sizes));
            }
            catch (Exception e)
            {
                string err = string.Format("PictureServiceHost.FSScalePicFileListRequest error = {0}, request = {1}", e.ToString(), JsonUtil<FSScalePicFileListRequest>.ToJson(request));
                LogUtil.Error(err);
            }

            return response;
        }


        /// <summary>
        /// 有效性校验
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private int ImageFileRequestValid(FSScalePicFileRequest request)
        {
            var errCode = (int)FS_ErrorCode.Request_Success;

            if (string.IsNullOrEmpty(request.file_guid) || request.width == 0 || request.height == 0)
            {
                LogUtil.Info("Error: request parameters are lost");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }

            return errCode;
        }

        private static int ImageFilesValid(FSScalePicFileListRequest request)
        {
            var errCode = (int)FS_ErrorCode.Request_Success;
            var sizes = request.sizes;
            
            if(string.IsNullOrEmpty(sizes))
            {
                LogUtil.Info("request parameters are lost");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }

            var sizeModel = JsonUtil<List<string>>.FromJosn(sizes);
            if (sizeModel == null || sizeModel.Count == 0)
            {
                LogUtil.Info("request parameters are lost");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }
            if (sizeModel != null && sizeModel.Count >= 5)
            {
                LogUtil.Info("request fileSize are too much");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }

            foreach (var str in sizeModel)
            {
                if (!str.Contains("_"))
                {
                    errCode = (int)FS_ErrorCode.RequestParam_Err;
                    LogUtil.Info("request parameters are lost");
                    break;
                }
                if (str.Split('_').Length <= 2) continue;
                errCode = (int)FS_ErrorCode.RequestParam_Err;
                LogUtil.Info("request parameters are lost");
                break;
            }

            return errCode;
        }

        private static int ImageFilesValid(FSAsyncScalePicFileListRequest request)
        {
            var errCode = (int)FS_ErrorCode.Request_Success;
            var sizes = request.sizes;

            if (string.IsNullOrEmpty(sizes))
            {
                LogUtil.Info("request parameters are lost");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }

            var sizeModel = JsonUtil<List<string>>.FromJosn(sizes);
            if (sizeModel == null || sizeModel.Count == 0)
            {
                LogUtil.Info("request parameters are lost");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }
            if (sizeModel != null && sizeModel.Count >= 5)
            {
                LogUtil.Info("request fileSize are too much");
                errCode = (int)FS_ErrorCode.RequestParam_Err;
            }

            foreach (var str in sizeModel)
            {
                if (!str.Contains("_"))
                {
                    errCode = (int)FS_ErrorCode.RequestParam_Err;
                    LogUtil.Info("request parameters are lost");
                    break;
                }
                if (str.Split('_').Length <= 2) continue;

                errCode = (int)FS_ErrorCode.RequestParam_Err;
                LogUtil.Info("request parameters are lost");
                break;
            }

            return errCode;
        }
    }
}