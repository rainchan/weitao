namespace WT.Services.Fs.Repository.Database
{
    public class FSDbFactory
    {
        private const string db_conn_write_key = "write_connection_string";
        private const string db_conn_read_key = "read_connection_string";

        private static FilesDA ReadFiles;
        private static FilesDA WriteFiles;

        private static FilePicDA ReadPics;
        private static FilePicDA WrirwPics;

        private static object obj = new object();

        //通用文件读
        public static IFilesDA CreateReadFSDB()
        {
            if (ReadFiles == null)
            {
                lock (obj)
                {
                    if(ReadFiles == null)
                        ReadFiles = new FilesDA(db_conn_read_key);
                }
            }

            return ReadFiles;
        }
        //通用文件写
        public static IFilesDA CreateWriteFSDB()
        {
            if (WriteFiles == null)
            {
                lock (obj)
                {
                    if (WriteFiles == null)
                        WriteFiles = new FilesDA(db_conn_write_key);
                }
            }

            return WriteFiles;
        }

        //图片文件读
        public static IFilePicDA CreatePicReadFSDB()
        {
            if (ReadPics == null)
            {
                lock (obj)
                {
                    if (ReadPics == null)
                    ReadPics = new FilePicDA(db_conn_read_key);
                }
            }

            return ReadPics;
        }
        //图片文件写
        public static IFilePicDA CreatePicWriteFSDB()
        {
            if (WrirwPics == null)
            {
                lock (obj)
                {
                    if (WrirwPics == null)
                    WrirwPics = new FilePicDA(db_conn_write_key);
                }
            }

            return WrirwPics;
        }
    }
}
