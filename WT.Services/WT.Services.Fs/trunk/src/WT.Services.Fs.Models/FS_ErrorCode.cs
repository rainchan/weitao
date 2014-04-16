using System.Collections.Generic;

namespace WT.Services.Fs.Models
{
    /// <summary>
    /// FS错误码
    /// </summary>
    public enum FS_ErrorCode
    {
        /// <summary>
        ///  ok
        /// </summary>
        Request_Success = 200,

        /// <summary>
        /// source file not exits
        /// </summary>
        RequestFile_Null = 301,

        /// <summary>
        /// source file error , can`t parse
        /// </summary>
        RequestFile_Name_Null = 302,

        /// <summary>
        /// file illegal
        /// </summary>
        RequestFile_Illegal = 303,
        
        /// <summary>
        /// file stream lost bytes
        /// </summary>
        RequestFile_Size_Invalid = 304,

        /// <summary>
        ///  parameters  error
        /// </summary>
        RequestParam_Err = 305,

        /// <summary>
        /// push message to RabbitMQ error
        /// </summary>
        PushToMQError = 306,

        /// <summary>
        ///  push message system error
        /// </summary>
        SystemError = 307,

        /// <summary>
        /// file upload error
        /// </summary>
        RequestFile_UploadErr = 501,

        /// <summary>
        /// time out
        /// </summary>
        RequestFile_TimeOut = 502,

        /// <summary>
        /// download error
        /// </summary>
        Download_err = 503,

        /// <summary>
        /// amr file convert to mp3 error
        /// </summary>
        Convert_err = 504,

        /// <summary>
        /// RetroCode commend Config path is null
        /// </summary>
        RetroCodeConfig_Err = 506,

        /// <summary>
        /// RetroCode commend not return
        /// </summary>
        RetroCode_CommendErr = 507,

        /// <summary>
        /// not find the file on server
        /// </summary>
        DB_FilePath_Null = 401,
    }

    /// <summary>
    /// FS错误码哈希表
    /// </summary>
    public class ErrorCodeDic
    {
        private static ErrorCodeDic instance;
        private Dictionary<int, string> ErrCodeDescDic;// = new Dictionary<int, string>();

        public ErrorCodeDic()
        {
            this.ErrCodeDescDic = new Dictionary<int, string>();
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.Request_Success, "OK");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_Null, "sour file not exits");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_Name_Null, "source file error , can`t parse");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_Illegal, "file illegal");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_Size_Invalid, " file stream lost bytes");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestParam_Err, " parameters  error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.PushToMQError, " push message to RabbitMQ error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.SystemError, " push message system error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_UploadErr, " file upload error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RequestFile_TimeOut, " time out");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.Download_err, " download error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.Convert_err, " amr file convert to mp3 error");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RetroCodeConfig_Err, " RetroCode commend Config path is null,");
            this.ErrCodeDescDic.Add((int)FS_ErrorCode.RetroCode_CommendErr, " RetroCode commend not return ,");

            this.ErrCodeDescDic.Add((int)FS_ErrorCode.DB_FilePath_Null, " not find the file on server");
        }

        public static ErrorCodeDic GetInstance()
        {
            if (instance == null)
                instance = new ErrorCodeDic();
            return instance;
        }

        public string CodeMessage(int code)
        {
            return ErrCodeDescDic[code];
        }
    }
}
