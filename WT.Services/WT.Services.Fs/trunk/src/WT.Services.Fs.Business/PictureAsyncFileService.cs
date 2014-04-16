using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Script.Serialization;
using WT.Components.Common.Utility;
using WT.Services.Fs.Business.Interfaces;
using WT.Services.Fs.Models;

namespace WT.Services.Fs.Business
{
    /// <summary>
    /// 异步图片缩放实现
    /// </summary>
    public class PictureAsyncFileService :BaseService, IAsyncPictureFileService
    {

        private int _errCode;

        public PictureAsyncFileService()
        {
            _errCode = (int)FS_ErrorCode.Request_Success;
        }

        /// <summary>
        /// 异步缩放指定比例列表图片
        /// </summary>
        /// <param name="fileGuid">原始图片GUID</param>
        /// <param name="sizes">尺寸列表</param>
        /// <returns></returns>
        public AsyncScalePicFileListModel AsyncScalePictureFile(string fileGuid, List<string> sizes)
        {
            var model = new AsyncScalePicFileListModel();
            var scalePicDics = new Dictionary<string, string>();
            
            var entity = new ScalePicFileListMessageEntity
                {
                    SourcePicGuid = fileGuid,
                    ScalePicGuidDic = new Dictionary<string, string>()
                };

            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach (string size in sizes)
            {
                var width = int.Parse(size.Split('_')[0]);
                var height = int.Parse(size.Split('_')[1]);

                try
                {
                    var picScaleKey = string.Format("{0}_{1}_{2}", fileGuid, width, height);
                    //string newGuid = ScalePictureOperation.GetScalePicGuidFromCache(fileGuid, width, height);

                    //if (newGuid != null)
                    //{
                    //    scalePicDics.Add(picScaleKey, newGuid);
                    //    continue;
                    //}

                    string newGuid = GetShortGuid();
                    scalePicDics.Add(picScaleKey, newGuid);
                    entity.ScalePicGuidDic.Add(picScaleKey, newGuid);
                }
                catch (Exception e)
                {
                    _errCode = (int)FS_ErrorCode.SystemError;
                    LogUtil.Error(string.Format("PictureAsyncFileService.AsyncScalePictureFile error={0}, {1}",e.ToString(),fileGuid));
                }
            }
            watch.Stop();
            LogUtil.Info(string.Format("PictureAsyncFileService.AsyncScalePictureFile GetScalePicGuids use {0} ms", watch.ElapsedMilliseconds));

            watch = new Stopwatch();
            watch.Start();
            PushMessageToMQ(entity);
            watch.Stop();
            LogUtil.Info(string.Format("PictureAsyncFileService.AsyncScalePictureFile PushMessageToMQ use {0} ms", watch.ElapsedMilliseconds));
            model.ret = _errCode;
            model.message = ErrorCodeDic.GetInstance().CodeMessage(_errCode);
            model.PicGuidDics = scalePicDics;

            return model;
        }

        /// <summary>
        /// push 缩放图片列表消息体至MQ
        /// </summary>
        /// <param name="entity"></param>
        public void PushMessageToMQ(ScalePicFileListMessageEntity entity)
        {
            //var mq = ScalePicsMQFactory.Instance.CreateScalePicsInfoToMQ();

            //try
            //{
            //    mq.Open();

            //    var serialize = new JavaScriptSerializer();
            //    var message = serialize.Serialize(entity);
            //    var success = mq.SendMessage(message);
            //    var info = (success) ? string.Format("AsyncPictureFileService.PushMessageToMQ message={0} success", message) :
            //        string.Format("AsyncPictureFileService.PushMessageToMQ message={0} fail", message);
            //    LogUtil.Info(info);

            //    if (!success)
            //        _errCode = (int)FS_ErrorCode.PushToMQError;

            //    //mq.Close();
            //}
            //catch (Exception ex)
            //{
            //    _errCode = (int)FS_ErrorCode.SystemError;
            //    var err = string.Format("AsyncPictureFileService.PushMessageToMQ exception={0}", ex.ToString());
            //    LogUtil.Error(err);
            //}
        }
    }
}
