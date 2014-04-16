using WT.Services.Fs.Repository.Entity;

namespace WT.Services.Fs.Repository.Database
{
    public interface IFilePicDA
    {
        /// <summary>
        /// 根据原始图片guid和尺寸获取file_Pic表中的数据
        /// </summary>
        /// <param name="picGuid"></param>
        /// <returns></returns>
        ScalePictureFileEntity GetScalePictureEntityByGuidAndSize(string picGuid);

        /// <summary>
        /// 插入一条新的图片pic_guid 和 文件guid的关联信息
        /// </summary>
        /// <param name="file_fuid"></param>
        /// <param name="pic_guid"></param>
        /// <param name="create_time"></param>
        /// <returns></returns>
        int InsertNewPicFileRelation(string file_fuid, string pic_guid, long create_time);
    }
}
