using WT.Components.Common.Helpers;
using WT.Components.Common.Utility;
using WT.Components.FastDfs;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using WT.Services.Fs.Business.Interfaces;
using WT.Services.Fs.Models;
using WT.Services.Fs.Repository.Database;
using WT.Services.Fs.Repository.Entity;
using WT.Services.Fs.Repository.Redis;

namespace WT.Services.Fs.Business
{
    public class CommonFilesService : BaseService, ICommonFilesService
    {
        private int errorCode;

        private IFilesDA fileDA_W;
        private IFilesDA fileDA_R;

        public CommonFilesService()
        {
            fileDA_W = FSDbFactory.CreateWriteFSDB();
            fileDA_R = FSDbFactory.CreateReadFSDB();
            errorCode = (int)FS_ErrorCode.Request_Success;
        }

        public AsyncsFileUploadResponse AsynFileUpload(string fileName, byte[] bytes)
        {
            if (!RequestValid(fileName, bytes))
                return new AsyncsFileUploadResponse { ret = errorCode, message = ErrorCodeDic.GetInstance().CodeMessage(errorCode), File_guid = string.Empty };

            AsyncsFileUploadResponse response = new AsyncsFileUploadResponse();
            try
            {
                string filePath = string.Empty;
                string fileExtName = (fileName.Split('.').Length == 1) ? "" : fileName.Split('.')[1];
                string fileGuid = GetShortGuid();
                int file_size = bytes.Length;

                // 计算MD5值， 若相同则不传物理文件至FDFS，只保存一条文件记录
                String fileMD5 = FileOperationHelper.CalculateChunkMD5(bytes);
                FileEntity fileEntity = fileDA_R.GetFileEntityByMD5(fileMD5);

                if (fileEntity != null && fileEntity.file_md5.Equals(fileMD5))  // 文件已存在
                {
                    FileEntity newEntity = new FileEntity
                    {
                        file_guid = fileGuid,
                        file_full_name = fileName,
                        file_path = fileEntity.file_path,
                        file_md5 = fileMD5,
                        file_size = file_size,
                        create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                        last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                    };
                   fileDA_W.CreateNewFile(newEntity);
                }
                else
                {
                    // 异步上传新文件
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    FSManager.BeginUploadFile(bytes, fileName, fileExtName, file_size, (uploadResult, uploadEx) =>
                    {
                        if (null != uploadEx)
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback: {0}", uploadEx.ToString()));
                            return;
                        }
                        if (null == uploadResult || string.IsNullOrEmpty(uploadResult.FilePathOnFDFS))
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback:fdfs file Pash is null. FileName={0},fileSize={1}", fileName, bytes.Length));
                            return;
                        }

                        watch.Stop();
                        string info = string.Format("Async upload at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4}, file_path = {5},"
                                    + " file_md5 = {6}, file_size = {7}, invoking time = {8}ms", Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId,
                                    Process.GetCurrentProcess().Threads.Count, fileGuid, fileName, uploadResult.FilePathOnFDFS, fileMD5, bytes.Length, watch.ElapsedMilliseconds);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);

                        // 保存FileGuid ,filePath 和FileName, FileSize到数据库      
                        watch.Start();
                        FileEntity entity = new FileEntity
                        {
                            file_guid = fileGuid,
                            file_full_name = fileName,
                            file_path = uploadResult.FilePathOnFDFS,
                            file_md5 = fileMD5,
                            file_size = file_size,
                            create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                            last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                        };
                        fileDA_W.CreateNewFile(entity);
                        watch.Stop();
                        info = string.Format("after async upload: Save to mysql db cost {0}ms. file_guid={1}, file_full_name={2}", watch.ElapsedMilliseconds, entity.file_guid, entity.file_full_name);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);
                    });
                }

                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = fileGuid;
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.RequestFile_UploadErr;
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = string.Empty;
            }
            return response;
        }

        public AsyncsFileUploadResponse AsynFileUploadWithGuid(string guid, string fileName, byte[] bytes)
        {
            if (!RequestValid(fileName, bytes))
                return new AsyncsFileUploadResponse { ret = errorCode, message = ErrorCodeDic.GetInstance().CodeMessage(errorCode), File_guid = string.Empty };

            AsyncsFileUploadResponse response = new AsyncsFileUploadResponse();
            try
            {
                string filePath = string.Empty;
                string fileExtName = (fileName.Split('.').Length == 1) ? "" : fileName.Split('.')[1];
                string fileGuid = guid;
                int file_size = bytes.Length;

                // 计算MD5值， 若相同则不传物理文件至FDFS，只保存一条文件记录
                String fileMD5 = FileOperationHelper.CalculateChunkMD5(bytes);
                FileEntity fileEntity = fileDA_R.GetFileEntityByMD5(fileMD5);

                if (fileEntity != null && fileEntity.file_md5.Equals(fileMD5))  // 文件已存在
                {
                    FileEntity newEntity = new FileEntity
                    {
                        file_guid = fileGuid,
                        file_full_name = fileName,
                        file_path = fileEntity.file_path,
                        file_md5 = fileMD5,
                        file_size = file_size,
                        create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                        last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                    };
                    fileDA_W.CreateNewFile(newEntity);
                }
                else
                {
                    // 异步上传新文件
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    FSManager.BeginUploadFile(bytes, fileName, fileExtName, file_size, (uploadResult, uploadEx) =>
                    {
                        if (null != uploadEx)
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback: {0}", uploadEx.ToString()));
                            return;
                        }
                        if (null == uploadResult || string.IsNullOrEmpty(uploadResult.FilePathOnFDFS))
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback:fdfs file Pash is null. FileName={0},fileSize={1}", fileName, bytes.Length));
                            return;
                        }

                        watch.Stop();
                        string info = string.Format("Async upload at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4}, file_path = {5},"
                                    + " file_md5 = {6}, file_size = {7}, invoking time = {8}ms", Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId,
                                    Process.GetCurrentProcess().Threads.Count, fileGuid, fileName, uploadResult.FilePathOnFDFS, fileMD5, bytes.Length, watch.ElapsedMilliseconds);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);

                        // 保存FileGuid ,filePath 和FileName, FileSize到数据库      
                        watch.Start();
                        FileEntity entity = new FileEntity
                        {
                            file_guid = fileGuid,
                            file_full_name = fileName,
                            file_path = uploadResult.FilePathOnFDFS,
                            file_md5 = fileMD5,
                            file_size = file_size,
                            create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                            last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                        };
                        fileDA_W.CreateNewFile(entity);
                        watch.Stop();
                        info = string.Format("after async upload: Save to mysql db cost {0}ms. file_guid={1}, file_full_name={2}", watch.ElapsedMilliseconds, entity.file_guid, entity.file_full_name);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);
                    });
                }

                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = fileGuid;
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.RequestFile_UploadErr;
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = string.Empty;
            }
            return response;
        }

       
        public AsyncsFileUploadResponse AsynAmrFileUpload(string fileName, string fileGuid, byte[] bytes)
        {
            if (!RequestValid(fileName, bytes))
                return new AsyncsFileUploadResponse { ret = errorCode, message = ErrorCodeDic.GetInstance().CodeMessage(errorCode), File_guid = string.Empty };

            AsyncsFileUploadResponse response = new AsyncsFileUploadResponse();
            try
            {
                string filePath = string.Empty;
                string fileExtName = (fileName.Split('.').Length == 1) ? "" : fileName.Split('.')[1];
                //string fileGuid = GetShortGuid();
                int file_size = bytes.Length;

                // 计算MD5值， 若相同则不传物理文件至FDFS，只保存一条文件记录
                String fileMD5 = FileOperationHelper.CalculateChunkMD5(bytes);
                FileEntity fileEntity = fileDA_R.GetFileEntityByMD5(fileMD5);

                if (fileEntity != null && fileEntity.file_md5.Equals(fileMD5))  // 文件已存在
                {
                    FileEntity newEntity = new FileEntity
                    {
                        file_guid = fileGuid,
                        file_full_name = fileName,
                        file_path = fileEntity.file_path,
                        file_md5 = fileMD5,
                        file_size = file_size,
                        create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                        last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                    };
                    long n = fileDA_W.CreateNewFile(newEntity);
                    LogUtil.Info(string.Format("CommonFilesService.AsynAmrFileUpload exits file fileEntity: {0} create result:{1} ",JsonUtil<FileEntity>.ToJson(fileEntity), n));
                }
                else
                {
                    LogUtil.Info(string.Format("CommonFilesService.AsynAmrFileUpload begin upload: {0} fileguid = {1}, bytes Length={2}", fileName, fileGuid, bytes.Length));
                    // 异步上传新文件
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    FSManager.BeginUploadFile(bytes, fileName, fileExtName, file_size, (uploadResult, uploadEx) =>
                    {
                        if (null != uploadEx)
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback: {0}", uploadEx.ToString()));
                            return;
                        }
                        if (null == uploadResult || string.IsNullOrEmpty(uploadResult.FilePathOnFDFS))
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback:fdfs file Pash is null. FileName={0},fileSize={1}", fileName, bytes.Length));
                            return;
                        }

                        watch.Stop();
                        string info = string.Format("Async upload at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4}, file_path = {5},"
                                    + " file_md5 = {6}, file_size = {7}, invoking time = {8}ms", Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId,
                                    Process.GetCurrentProcess().Threads.Count, fileGuid, fileName, uploadResult.FilePathOnFDFS, fileMD5, bytes.Length, watch.ElapsedMilliseconds);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);

                        // 保存FileGuid ,filePath 和FileName, FileSize到数据库      
                        watch.Start();
                        FileEntity entity = new FileEntity
                        {
                            file_guid = fileGuid,
                            file_full_name = fileName,
                            file_path = uploadResult.FilePathOnFDFS,
                            file_md5 = fileMD5,
                            file_size = file_size,
                            create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                            last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                        };
                        fileDA_W.CreateNewFile(entity);
                        watch.Stop();
                        info = string.Format("after async upload: Save to mysql db cost {0}ms. file_guid={1}, file_full_name={2}", watch.ElapsedMilliseconds, entity.file_guid, entity.file_full_name);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            LogUtil.Error(info);
                            errorCode = (int)FS_ErrorCode.RequestFile_TimeOut;
                        }
                        else
                            LogUtil.Info(info);
                    });
                }

                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = fileGuid;
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.RequestFile_UploadErr;
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = string.Empty;
            }
            return response;
        }

        public AsyncFileDownloadResponse AsynFileDownload(string file_guid)
        {
            if (string.IsNullOrEmpty(file_guid))
            {
                errorCode = (int)FS_ErrorCode.RequestParam_Err;
                return new AsyncFileDownloadResponse
                {
                    ret = errorCode,
                    message = ErrorCodeDic.GetInstance().CodeMessage(errorCode),
                    File_url = ""
                };
            }

            AsyncFileDownloadResponse response = new AsyncFileDownloadResponse();
            try
            {
                // 根据FileGuid 查询FilePath
                ICommonFileRedis Ra = CommonFileRedisFactory.CreateWriteFilesRedis();
                string filePath = Ra.GetURLByGuid(file_guid);

                if (filePath == null)
                {
                    FileEntity entity = fileDA_R.GetFileEntityByGuid(file_guid);
                    
                    if (entity == null)
                    {
                        errorCode = (int)FS_ErrorCode.DB_FilePath_Null;
                        response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                        return response;
                    }
                    filePath = entity.file_path;

                    ICommonFileRedis RaW = CommonFileRedisFactory.CreateWriteFilesRedis();
                    RaW.SetCommonFileGuidUrl(file_guid, filePath);
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
                LogUtil.Info(string.Format("CommonFilesService.AsynFileDownload {0}",JsonUtil<AsyncFileDownloadResponse>.ToJson(response)));
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

        public FileUploadResponse FileUpload(string fileName, long file_size, byte[] file_buffer)
        {
            FileUploadResponse response = new FileUploadResponse();
            try
            {
                string filePath = string.Empty;
                string fileExtName = fileName.Split('.')[1];
                string fileGuid = GetShortGuid();

                // 计算MD5值， 若相同则不传物理文件至FDFS，只保存一条文件记录
                String fileMD5 = FileOperationHelper.CalculateChunkMD5(file_buffer);
                FileEntity fileEntity = fileDA_R.GetFileEntityByMD5(fileMD5);

                if (fileEntity != null && fileEntity.file_md5.Equals(fileMD5))  // 文件已存在
                {
                    FileEntity newEntity = new FileEntity
                    {
                        file_guid = fileGuid,
                        file_full_name = fileName,
                        file_path = fileEntity.file_path,
                        file_md5 = fileMD5,
                        file_size = file_size,
                        create_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                    };
                    fileDA_W.CreateNewFile(newEntity);
                }
                else
                {
                    // 异步上传新文件
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    FSManager.BeginUploadFile(file_buffer, fileName, fileExtName, file_size, (uploadResult, uploadEx) =>
                    {
                        if (null != uploadEx)
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback: {0}", uploadEx.ToString()));
                            errorCode = (int)FS_ErrorCode.RequestFile_UploadErr;
                            return;
                        }
                        if (null == uploadResult || string.IsNullOrEmpty(uploadResult.FilePathOnFDFS))
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback:fdfs file Pash is null. FileName={0},fileSize={1}", fileName, file_size));
                            errorCode = (int)FS_ErrorCode.RequestFile_UploadErr;
                            return;
                        }

                        watch.Stop();
                        string info = string.Format("Async upload at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4},"
                                    +" file_path = {5}, file_md5 = {6}, file_size = {7}, invoking time = {8}ms", Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, Process.GetCurrentProcess().Threads.Count, fileGuid, fileName, uploadResult.FilePathOnFDFS, fileMD5, file_size, watch.ElapsedMilliseconds);
                        if (watch.ElapsedMilliseconds > 3000)
                            LogUtil.Error(info);
                        else
                            LogUtil.Info(info);


                        // 保存FileGuid ,filePath 和FileName, FileSize到数据库      
                        watch.Start();
                        FileEntity entity = new FileEntity
                        {
                            file_guid = fileGuid,
                            file_full_name = fileName,
                            file_path = uploadResult.FilePathOnFDFS,
                            file_md5 = fileMD5,
                            file_size = file_size,
                            create_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                        };
                        fileDA_W.CreateNewFile(entity);
                        watch.Stop();
                        info = string.Format("after async upload: Save to mysql db cost {0}ms. file_guid={1}, file_full_name={2}", watch.ElapsedMilliseconds, entity.file_guid, entity.file_full_name);
                        if (watch.ElapsedMilliseconds > 3000)
                            LogUtil.Error(info);
                        else
                            LogUtil.Info(info);
                    });
                }

                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = fileGuid;
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.RequestParam_Err;
                response.ret = errorCode;
                response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                response.File_guid = string.Empty;
            }
            return response;
        }

        public FileDownloadResponse FileDownload(string file_guid)
        {
            FileDownloadResponse response = new FileDownloadResponse();
            try
            {
                // 根据FileGuid 查询FilePath
                FileEntity entity = fileDA_R.GetFileEntityByGuid(file_guid);
                if (entity == null)
                {
                    return InitResponse((int)FS_ErrorCode.DB_FilePath_Null, string.Empty, null);
                }

                string filePath = entity.file_path;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                FSManager.DownloadFile(filePath, (ex, fileBufferOnFDFS) =>
                {
                    if (null != ex)
                    {
                        LogUtil.Error(string.Format("Error in FSManager.DownloadFile callback: {0}", ex.ToString()));
                        response = InitResponse(errorCode,entity.file_full_name, null);
                        errorCode = (int)FS_ErrorCode.Download_err;
                        response.File_buffer = null;
                        response.ret = errorCode;
                        response.message = ErrorCodeDic.GetInstance().CodeMessage(errorCode);
                        return;
                    }

                    response = InitResponse(errorCode, entity.file_full_name, fileBufferOnFDFS);
                    
                    watch.Stop();
                    string info = string.Format("sync download at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4}, file_path = {5},"
                                + " file_md5 = {6}, file_size = {7}, invoking time = {8}ms", Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, Process.GetCurrentProcess().Threads.Count, file_guid, entity.file_full_name, entity.file_path, entity.file_md5, entity.file_size, watch.ElapsedMilliseconds);
                    LogUtil.Info(info);
                });
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(FileDownloadRequest): {0}", ex.ToString()));
                errorCode = (int)FS_ErrorCode.Download_err;

                response = InitResponse(errorCode, string.Empty, null);
            }

            return response;
        }

        private FileDownloadResponse InitResponse(int code, string fileName, byte[] buffer)
        {
            FileDownloadResponse response = new FileDownloadResponse();
            response.ret = code;
            response.message = ErrorCodeDic.GetInstance().CodeMessage(code);
            response.File_name = fileName;
            response.File_buffer = buffer;

            return response;
        }

        /// <summary>
        /// 文件流异步上传有效性校验
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private bool RequestValid(string fileName, byte[] bytes)
        {
            bool isValid = false;

            if (string.IsNullOrEmpty(fileName))
            {

                LogUtil.Error("Error: request file FileName is null.");
                errorCode = (int)FS_ErrorCode.RequestFile_Name_Null;
                return isValid;
            }
            if (fileName.Split('.').Length != 2)
            {
                LogUtil.Error("Error: request file illegal");
                errorCode = (int)FS_ErrorCode.RequestFile_Illegal;
                return isValid;
            }

            isValid = true;
            return isValid;
        }

    }
}
