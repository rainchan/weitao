using WT.Services.Fs.Repository.Entity;

namespace WT.Services.Fs.Repository.Database
{
    public interface IFilesDA
    {
        /// <summary>
        ///FDFS创建新文件
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        long CreateNewFile(FileEntity entity);

        /// <summary>
        /// 根据Guid值查询一条记录
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <returns></returns>
        FileEntity GetFileEntityByGuid(string fileGuid);

        /// <summary>
        /// 根据MD5值查询一条记录
        /// </summary>
        /// <param name="fileMD5"></param>
        /// <returns></returns>
        FileEntity GetFileEntityByMD5(string fileMD5);
    }
}
