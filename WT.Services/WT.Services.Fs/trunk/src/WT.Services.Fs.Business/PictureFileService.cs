using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WT.Components.Common.Utility;
using WT.Components.FastDfs.Entities;
using WT.Services.Fs.Business.Interfaces;
using WT.Services.Fs.Models;
using WT.Services.Fs.Repository.Database;
using WT.Services.Fs.Repository.Entity;

namespace WT.Services.Fs.Business
{
    /// <summary>
    /// 图片文件处理请求
    /// </summary>
    public class PictureFileService:BaseService,IPictureFileService
    {
        private int errorCode { get; set; }
        private byte[] bytes { get; set; }

        public PictureFileService()
        {
            errorCode = (int)FS_ErrorCode.Request_Success;
            bytes = null;
        }

        public AsyncFileDownloadResponse ScalePictureFile(string file_guid, int width, int height)
        {
            AsyncFileDownloadResponse response = new AsyncFileDownloadResponse();
            try
            {
                string picGuid = string.Format("{0}_{1}_{2}", file_guid, width, height);
                string filePath = ScalePictureOperation.GetFileURLFromCache(file_guid, width, height);

                if (filePath == null)
                {
                    byte[] bytes = ScalePictureOperation.GetOriginlPicBytes(file_guid);

                    if (bytes == null)
                    {
                        errorCode = (int)FS_ErrorCode.DB_FilePath_Null;

                        response.ret = errorCode;
                        response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                        response.File_url = "";
                        return response;
                    }

                    FSImageEntity imageEntity = null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(bytes, 0, bytes.Length);
                        imageEntity = new FSImageEntity(Image.FromStream(ms), width, height);
                        imageEntity.ConvertSourcePic();
                    }

                    FileEntity newEntity = ScalePictureOperation.PostNewPicture(picGuid + ".jpg", imageEntity.JpegBytes);

                    IFilePicDA fileDA = FSDbFactory.CreatePicWriteFSDB();
                    fileDA.InsertNewPicFileRelation(newEntity.file_guid, picGuid, StringUtil.GetDateTimeSeconds(DateTime.Now));

                    filePath = newEntity.file_path;
                }

                Hashtable nginxService = System.Configuration.ConfigurationManager.GetSection("NginxService") as Hashtable;
                string address = nginxService["Address"].ToString();

                if (string.IsNullOrEmpty(filePath))
                {
                    errorCode = (int)FS_ErrorCode.DB_FilePath_Null;
                }
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_url = address + filePath;
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(FileDownloadRequest): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.RequestParam_Err;
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
            }
            return response;
        }

        public ScalePicFileListModel ScalePictureFile(string file_guid, List<string> sizes)
        {
            ScalePicFileListModel response = new ScalePicFileListModel();
            response.picUrls = new Dictionary<string, string>();

            #region
            Hashtable nginxService = System.Configuration.ConfigurationManager.GetSection("NginxService") as Hashtable;
            string address = nginxService["Address"].ToString();

            foreach (string size in sizes)
            {
                int width = int.Parse(size.Split('_')[0]);
                int height = int.Parse(size.Split('_')[1]);

                try
                {
                    string picGuid = string.Format("{0}_{1}_{2}", file_guid, width, height);
                    string filePath = ScalePictureOperation.GetFileURLFromCache(file_guid, width, height);

                    if (filePath == null)
                    {
                        if(bytes == null)
                            bytes = ScalePictureOperation.GetOriginlPicBytes(file_guid);

                        if (bytes == null)
                        {
                            errorCode = (int)FS_ErrorCode.DB_FilePath_Null;
                            response.ret = errorCode;
                            response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                            break;
                        }
                        FSImageEntity imageEntity = null;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(bytes, 0, bytes.Length);
                            imageEntity = new FSImageEntity(Image.FromStream(ms), width, height);
                            imageEntity.ConvertSourcePic();
                        }

                        FileEntity newEntity = ScalePictureOperation.PostNewPicture(picGuid + ".jpg", imageEntity.JpegBytes);

                        IFilePicDA fileDA = FSDbFactory.CreatePicWriteFSDB();
                        fileDA.InsertNewPicFileRelation(newEntity.file_guid, picGuid, StringUtil.GetDateTimeSeconds(DateTime.Now));

                        filePath = newEntity.file_path;
                    }

                    if (string.IsNullOrEmpty(filePath))
                    {
                        errorCode = (int)FS_ErrorCode.DB_FilePath_Null;
                        LogUtil.Info(string.Format("ScalePictureFile GetPicFileList size: {0} is not in FDFS", size));
                    }
                    
                    response.picUrls.Add(size, address + filePath);
                }
                catch (Exception ex)
                {
                    LogUtil.Error(string.Format("Exception FSService.Post(ScalePictureFile GetPicFileList): {0}", ex.ToString()));
                    errorCode = (int)FS_ErrorCode.RequestParam_Err;
                    response.ret = errorCode;
                    response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                }
            }
            #endregion

            response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
            return response;
            
        }
    }
}
