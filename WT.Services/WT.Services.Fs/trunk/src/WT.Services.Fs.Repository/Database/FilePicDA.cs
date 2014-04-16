using WT.Components.Common.Utility;
using ServiceStack.OrmLite;
using System;
using System.Data;
using WT.Services.Fs.Repository.Entity;

namespace WT.Services.Fs.Repository.Database
{
    public class FilePicDA : FSDbInstance, IFilePicDA
    {
        public FilePicDA() : base() { 
        
        }

        public FilePicDA(string conn) : base(conn) { 
        
        }

        public ScalePictureFileEntity GetScalePictureEntityByGuidAndSize(string picGuid)
        {
            ScalePictureFileEntity entity = null;
            try
            {
                using (IDbConnection dbCon = dbFactory.OpenDbConnection())
                {
                    string sql = string.Format("select Id, file_guid, pic_guid, create_time from file_pics where pic_guid = {0}".Params(picGuid));
                    entity = dbCon.QuerySingle<ScalePictureFileEntity>(sql);
                }
            }
            catch (Exception e)
            {
                string err = string.Format("DataBase Exception in GetScalePictureEntityByGuidAndSize(): {0}, pic_guid ={1}", e.ToString(), picGuid);
                LogUtil.Error(err);
            }

            return entity;
        }
    
        public int InsertNewPicFileRelation(string file_fuid, string pic_guid, long create_time)
        {
            int result = 0;
            try
            {
                using (IDbConnection dbCon = dbFactory.OpenDbConnection())
                {
                    string sql = string.Format("insert into file_pics(file_guid, pic_guid, create_time) values({0},{1},{2})".Params(file_fuid,
                        pic_guid, create_time));
                    result = dbCon.ExecuteSql(sql);
                }
            }
            catch (Exception e)
            {
                string err = string.Format("DataBase Exception in InsertNewPicFileRelation(): {0}, file_guid={1}, pic_guid={2}", e.ToString(), file_fuid, pic_guid);
                LogUtil.Error(err);
            }

            return result;
        }
}
}
