using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WT.Components.Common.Helpers
{
    public class FileOperationHelper
    {
        public static long CHUNK_SIZE = 2 * 1024 * 1024;   // 文件块最大容量：2M

        public static void ReadFileBytes(string filePath, out string fileExt, out long fileSize, out byte[] fileBytes)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            fileExt = fileInfo.Extension;
            FileStream stream = fileInfo.OpenRead();
            fileSize = stream.Length;
            fileBytes = new byte[fileSize];
            stream.Read(fileBytes, 0, (int)fileSize);
            stream.Close();
        }

        public static void ReadFileBytes(string filePath, long bp, long ep, out byte[] fileBytes)
        {
            int count = (int)ep - (int)bp;
            fileBytes = new byte[count];
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream stream = fileInfo.OpenRead();
            stream.Seek(bp, SeekOrigin.Begin);
            stream.Read(fileBytes, (int)bp, count);
            stream.Close();
        }

        public static byte[] ReadFileBytes(FileInfo fileInfo, long bp, long ep)
        {
            int count = (int)ep - (int)bp;
            byte[] fileBytes = new byte[count];
            FileStream stream = fileInfo.OpenRead();
            stream.Seek(bp, SeekOrigin.Begin);
            stream.Read(fileBytes, 0, count);
            stream.Close();
            return fileBytes;
        }

        public static void WriteFileBytes(string filePath, byte[] bytes, int offset)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            FileStream fileStream = new FileStream(filePath, FileMode.Append);
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(bytes, 0, bytes.Length); //将字节数据写入文件
            fileStream.Close();
        }

        public static string CreateFile(string selectDirectory, string fileName)
        {
            if (string.IsNullOrEmpty(selectDirectory))
            {
                selectDirectory = Directory.GetCurrentDirectory();
            }

            string filePath = string.Format("{0}{1}{2}", selectDirectory,Path.AltDirectorySeparatorChar, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.Create(filePath);

            return filePath;
        }

        public static void CalculateChunkMD5(FileInfo fileInfo, out Dictionary<int, string> md5Dic, out string fileMD5)
        {
            double temp = (double)fileInfo.Length / CHUNK_SIZE;
            int chunkNumber = (int)Math.Ceiling(temp);
            md5Dic = new Dictionary<int, string>();
            fileMD5 = string.Empty;
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

            for (int i = 0; i < chunkNumber; i++)
            {
                string chunkMD5 = string.Empty;
                if (i == chunkNumber - 1)
                {
                    long tailSize = fileInfo.Length % CHUNK_SIZE;
                    byte[] bytes = new byte[tailSize];
                    fs.Read(bytes, 0, (int)tailSize);
                    chunkMD5 = CalculateChunkMD5(bytes);
                }
                else
                {
                    byte[] chunkBytes = new byte[CHUNK_SIZE];
                    fs.Read(chunkBytes, 0, (int)CHUNK_SIZE);
                    chunkMD5 = CalculateChunkMD5(chunkBytes);

                }
                md5Dic.Add(i, chunkMD5);
            }
            fs.Close();

            StringBuilder sb = new StringBuilder(string.Empty);
            foreach (string chunkMD5 in md5Dic.Values)
            {
                sb.Append(chunkMD5);
            }
            fileMD5 = CalculateMD5(sb.ToString());
        }

        public static string CalculateChunkMD5(byte[] chunkBytes)
        {
            StringBuilder MD5Value = new StringBuilder(64);
            MD5CryptoServiceProvider md5_provider = new MD5CryptoServiceProvider();
            byte[] data_md5 = md5_provider.ComputeHash(chunkBytes, 0, chunkBytes.Length);
            foreach (byte b in data_md5)
            {
                MD5Value.Append(b.ToString("X2"));
            }
            return MD5Value.ToString();
        }

        public static string CalculateMD5(FileInfo fileInfo)
        {
            StringBuilder MD5Value = new StringBuilder(64);
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data_md5 = md5.ComputeHash(fs);
            foreach (byte b in data_md5)
            {
                MD5Value.Append(b.ToString("X2"));
            }
            fs.Close();

            return MD5Value.ToString();
        }

        public static string CalculateMD5(string str)
        {
            StringBuilder MD5Value = new StringBuilder(64);
            MD5CryptoServiceProvider md5_provider = new MD5CryptoServiceProvider();
            byte[] data_md5 = md5_provider.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (byte b in data_md5)
            {
                MD5Value.Append(b.ToString("X2"));
            }
            return MD5Value.ToString();
        }
    }
}
