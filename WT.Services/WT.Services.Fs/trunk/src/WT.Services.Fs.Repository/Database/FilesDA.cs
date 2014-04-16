using System;
using System.Data;
using ServiceStack.OrmLite;
using WT.Components.Common.Utility;
using WT.Services.Fs.Repository.Entity;

namespace WT.Services.Fs.Repository.Database
{
    public class FilesDA:FSDbInstance,IFilesDA
    {
        public FilesDA() : base() {
        
        }

        public FilesDA(string conn):base(conn){
        
        }

        #region IFilesDA 
        public long CreateNewFile(FileEntity entity)
        {
            int affectedRow = 0;

            try
            {
                using (IDbConnection dbCon = dbFactory.OpenDbConnection())
                {
                    string sql = string.Format("insert into autopia_files (file_guid, file_full_name, file_path, file_md5,"
                               + " file_size, create_time, last_changed_time) values ({0}, {1}, {2}, {3}, {4},{5}, {6})".Params(
                               entity.file_guid,
                               entity.file_full_name,
                               entity.file_path,
                               entity.file_md5,
                               entity.file_size,
                               entity.create_time,
                               entity.last_changed_time));
                    affectedRow = dbCon.ExecuteSql(sql);
                }
            }
            catch (Exception e)
            {
                string err = string.Format("DataBase Exception in CreateNewFile(): {0}, file_guid={1}, fileName={2}", e.ToString()+this.ConnectionString, entity.file_guid, entity.file_full_name);
                LogUtil.Error(err);
            }
            
            return affectedRow;

        }

        public FileEntity GetFileEntityByGuid(string fileGuid)
        {
            FileEntity entity = null;
            try {
                using (IDbConnection dbCon = dbFactory.OpenDbConnection())
                {
                    string sql = string.Format("select Id, file_guid, file_full_name, file_path, file_md5, file_size, create_time,"
                               + " last_changed_time from autopia_files where file_guid = {0}".Params(fileGuid));
                    entity = dbCon.QuerySingle<FileEntity>(sql);
                }
            }
            catch(Exception e)
            {
                string err = string.Format("DataBase Exception in GetFileEntityByGuid(): {0}, file_guid={1}", e.ToString(), fileGuid);
                LogUtil.Error(err);
            }

            return entity;
        }

        public FileEntity GetFileEntityByMD5(string fileMD5)
        {
            FileEntity entity = null;
            try
            {
                using (IDbConnection dbCon = dbFactory.OpenDbConnection())
                {
                    string sql = string.Format("select Id, file_guid, file_full_name, file_path, file_md5, file_size, create_time,"
                               + " last_changed_time from autopia_files where file_md5 = {0}".Params(fileMD5));
                    entity = dbCon.QuerySingle<FileEntity>(sql);
                }
            }
            catch (Exception e)
            {
                string err = string.Format("DataBase Exception in GetFileEntityByMD5(): {0}, file_md5={1}", e.ToString(), fileMD5);
                LogUtil.Error(err);
            }

            return entity;
        }
        #endregion
    }
}
