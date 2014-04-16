using WT.Components.Common.Helpers;
using WT.Components.FastDfs;
using System;
using System.Diagnostics;
using System.Threading;
using WT.Components.Common.Utility;
using WT.Services.Fs.Repository.Database;
using WT.Services.Fs.Repository.Entity;
using WT.Services.Fs.Repository.Redis;

namespace WT.Services.Fs.Business
{
    public class ScalePictureOperation
    {
       /// <summary>
        /// 缓存中获取文件地址
       /// </summary>
       /// <param name="file_guid">文件guid</param>
       /// <param name="width">宽</param>
       /// <param name="height">高</param>
       /// <returns>为空文件不存在</returns>
        public static string GetFileURLFromCache(string file_guid, int width, int height)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string value = GetFileUrlFromRedis(file_guid, width, height);

            if (value == null)
            {
                FileEntity entity = GetScaleFilePathFromDB(string.Format("{0}_{1}_{2}", file_guid, width, height));
                
                if (entity != null)
                {
                    value = entity.file_path;
                    IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateWriteFileRedis();
                    ra.SetScalePicFile(file_guid, width, height, value);
                }
            }
            watch.Stop();
            LogUtil.Info(string.Format("ScalePictureOperation.GetFileInfoFromCache invoking time {0}ms",watch.ElapsedMilliseconds));
            return value;
        }

        /// <summary>
        /// 从缓存中获取文件对象
        /// </summary>
        /// <param name="sourceFileGuid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static FileEntity GetFileEntityFromCache(string sourceFileGuid, int width, int height)
        {
            FileEntity entity = new FileEntity();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            entity = GetFileEntityFromRedis(sourceFileGuid, width, height);

            if (entity == null)
            {
                entity = GetScaleFilePathFromDB(string.Format("{0}_{1}_{2}", sourceFileGuid, width, height));

                if (entity != null)
                {
                    watch.Stop();
                    LogUtil.Info(string.Format("ScalePictureOperation.GetFileEntityFromDB{0} invoking time {1}ms",
                                               JsonUtil<FileEntity>.ToJson(entity), watch.ElapsedMilliseconds));
                    IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateWriteFileRedis();
                    ra.SetScalePicEntity<FileEntity>(sourceFileGuid, width, height, entity);
                }
            }
            else
            {
                watch.Stop();
                LogUtil.Info(string.Format("ScalePictureOperation.GetFileEntityFromRedis{0} invoking time {1}ms", JsonUtil<FileEntity>.ToJson(entity), watch.ElapsedMilliseconds));
            }
           
            return entity;
        }

        /// <summary>
        /// 缓存中获取文件guid
        /// </summary>
        /// <param name="file_guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetScalePicGuidFromCache(string file_guid, int width, int height)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string value = GetFileGuidFromRedis(file_guid, width, height);

            if (value == null)
            {
                FileEntity entity = GetScaleFilePathFromDB(string.Format("{0}_{1}_{2}", file_guid, width, height));
                if (entity != null)
                {
                    value = entity.file_guid;
                    IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateWriteFileRedis();
                    ra.SetScalePicGuid(file_guid, width, height, value);
                }
            }
            watch.Stop();
            LogUtil.Info(string.Format("ScalePictureOperation.GetScalePicGuidFromCache invoking time {0}ms", watch.ElapsedMilliseconds));
            return value;
        }

        /// <summary>
        /// 获取Redis中缩放图片GUID
        /// </summary>
        /// <param name="file_guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static string GetFileGuidFromRedis(string file_guid, int width, int height)
        {
            string guid = null;
            IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateReadFileRedis();
            guid = ra.GetScalePicGuid(file_guid, width, height);
            return guid;
        }

        /// <summary>
        /// 查询redis是否存在该文件
        /// </summary>
        /// <param name="file_guid">文件guid</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>为空表示redis不存在该文件</returns>
        private static string GetFileUrlFromRedis(string file_guid, int width, int height)
        {
            string url = null;
            IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateReadFileRedis();
            url = ra.GetScalePicURL(file_guid, width, height);
            return url;
        }

        /// <summary>
        /// 查询某尺寸文件的存放地址
        /// </summary>
        /// <param name="fileKey">guid_width_height</param>
        /// <returns>文件fdfs的路径，空表示文件不存在</returns>
        private static FileEntity GetScaleFilePathFromDB(string fileKey)
        {
            IFilePicDA picDA_R = FSDbFactory.CreatePicReadFSDB();

            ScalePictureFileEntity entity = picDA_R.GetScalePictureEntityByGuidAndSize(fileKey);
            string fileGuid = (entity == null) ? null : entity.file_guid;

            if (fileGuid == null)
                return null;

            IFilesDA fileDA = FSDbFactory.CreateReadFSDB();
            FileEntity fileEntity = fileDA.GetFileEntityByGuid(fileGuid);

            if (fileEntity == null)
                return null;

            return fileEntity;
        }

        /// <summary>
        /// 获取原始图片文件流
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>图片字节流， 为空未找到文件</returns>
        public static byte[] GetOriginlPicBytes(string guid)
        {
            byte[] file_buffer = null;

            try
            {
                // 根据FileGuid 查询FilePath
                IFilesDA fileDA = FSDbFactory.CreateReadFSDB();
                FileEntity entity = fileDA.GetFileEntityByGuid(guid);
                if (entity == null)
                {
                    LogUtil.Info(string.Format("ScalePictureOperation.GetOriginlPicBytes not found guid={0} in DB ",guid));
                    return file_buffer;
                }

                string filePath = entity.file_path;
                
                Stopwatch watch = new Stopwatch();
                watch.Start();
                FSManager.DownloadFile(filePath, (ex, fileBufferOnFDFS) =>
                {
                    if (null != ex)
                    {
                        LogUtil.Error(string.Format("Error in FSManager.DownloadFile callback: {0}", ex.ToString()));
                        return;
                    }

                    file_buffer = fileBufferOnFDFS;
                    watch.Stop();
                    string info = string.Format("sync download at server(ProcessId = {0}, ThreadId = {1}, CurrTheadsNum = {2}): file_guid = {3}, file_full_name = {4}, file_path = {5}, file_md5 = {6}, file_size = {7}, invoking time = {8}ms",
                        Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, Process.GetCurrentProcess().Threads.Count, guid, entity.file_full_name, entity.file_path, entity.file_md5, entity.file_size, watch.ElapsedMilliseconds);
                    LogUtil.Info(info);
                });
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(FileDownloadRequest): {0}", ex.ToString()));
            }

            return file_buffer;
        }

        /// <summary>
        /// 获取redis中文件缓存
        /// </summary>
        /// <param name="file_guid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static FileEntity GetFileEntityFromRedis(string file_guid, int width, int height)
        {
            FileEntity entity = new FileEntity();
            IScalePicFileRedis ra = ScalePicFileRedisFactory.CreateReadFileRedis();
            entity = ra.GetScalePicEntity<FileEntity>(file_guid, width, height);
            return entity;
        }

        /// <summary>
        /// 上传新文件并返回
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns>文件实体</returns>
        public static FileEntity PostNewPicture(string fileName, byte[] bytes)
        {
            string fileGuid = GetShortGuid();
            return PostNewPicture(fileGuid, fileName, bytes);
        }

        /// <summary>
        /// 获取16个字符的短guid
        /// </summary>
        /// <returns>sting</returns>
        private static string GetShortGuid()
        {
            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= (int)b + 1;
            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 上传新文件并指定guid
        /// </summary>
        /// <param name="guid">guid</param>
        /// <param name="fileName">文件名</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns></returns>
        public static FileEntity PostNewPicture(string guid, string fileName, byte[] bytes)
        {
            FileEntity fileEntity = null;
            try
            {
                string filePath = string.Empty;
                string fileExtName = (fileName.Split('.').Length == 1) ? string.Empty : fileName.Split('.')[1];

                // 计算MD5值， 若相同则不传物理文件至FDFS，只保存一条文件记录
                string fileMD5 = FileOperationHelper.CalculateChunkMD5(bytes);

                IFilesDA fileDA_R = FSDbFactory.CreateReadFSDB();
                fileEntity = fileDA_R.GetFileEntityByMD5(fileMD5);
                IFilesDA fileDA = FSDbFactory.CreateWriteFSDB();

                if (fileEntity != null && fileEntity.file_md5.Equals(fileMD5))
                {
                    FileEntity newEntity = new FileEntity
                    {
                        file_guid = guid,
                        file_full_name = fileName,
                        file_path = fileEntity.file_path,
                        file_md5 = fileMD5,
                        file_size = bytes.Length,
                        create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                        last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                    };
                    fileDA.CreateNewFile(newEntity);

                    fileEntity = newEntity;
                }
                else
                {
                    FSManager.UploadFile(bytes, fileExtName, (Exception, path) =>
                    {
                        if (null != Exception)
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback: {0}", Exception.ToString()));
                            return;
                        }

                        if (string.IsNullOrEmpty(path))
                        {
                            LogUtil.Error(string.Format("Error in FSManager.BeginUploadFile callback:fdfs file Pash is null. FileName={0},fileSize={1}", fileName, bytes.Length));
                            return;
                        }

                        FileEntity entity = new FileEntity
                        {
                            file_guid = guid,
                            file_full_name = fileName,
                            file_path = path,
                            file_md5 = fileMD5,
                            file_size = bytes.Length,
                            create_time = StringUtil.GetDateTimeSeconds(DateTime.Now),
                            last_changed_time = StringUtil.GetDateTimeSeconds(DateTime.Now)
                        };
                        fileDA.CreateNewFile(entity);
                        fileEntity = entity;
                    });
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("Exception FSService.Post(): {0}", ex.ToString()));
            }

            return fileEntity;
        }
    }
}
