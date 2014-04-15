using WT.Components.Common.Log.Mongo;
using log4net.Appender;
using log4net.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Linq;

namespace WT.Components.Common.Log
{
    public class MongoDBAppender : AppenderSkeleton
    {
        public MongoDBAppender()
        {
        }

        /// <summary>
        /// MongoDB database connection in the format:
        /// mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
        /// See http://www.mongodb.org/display/DOCS/Connections
        /// If no database specified, default to "log4net"
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the collection in database
        /// Defaults to "logs"
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Name of the database on MongoDB
        /// Defaults to log4net_mongodb
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override bool RequiresLayout
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 将loggingEvent对象转换成Bson对象存入Mongo
        /// </summary>
        /// <param name="loggingEvent">
        /// loggingEvent对象
        /// </param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            MongoServer mongo = null;
            try
            {
                mongo = GetMongo();
                mongo.Connect();

                MongoDatabase database = mongo.GetDatabase(DatabaseName ?? "logsDotNet");
                MongoCollection collection = database.GetCollection(CollectionName);
                collection.Insert(BuildBsonDocument(loggingEvent));
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("写入MonogoDb异常,ex={0}", ex.ToString()));
            }
            finally
            {
                if (mongo != null)
                {
                    mongo.Disconnect();
                }
            }
        }

        /// <summary>
        /// 将loggingEvent对象转换成Bson对象存入Mongo
        /// </summary>
        /// <param name="loggingEvent">
        /// loggingEvent对象
        /// </param>
        protected override void Append(LoggingEvent[] loggingEvents)
        {
            MongoServer mongo = null;
            try
            {
                mongo = GetMongo();
                mongo.Connect();
                MongoDatabase database = mongo.GetDatabase(DatabaseName ?? "logsDotNet");
                MongoCollection collection = database.GetCollection(CollectionName);
                collection.InsertBatch(loggingEvents.Select(BuildBsonDocument));
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("写入MonogoDb异常,ex={0}", ex.ToString()));
            }
            finally
            {
                if (mongo != null)
                {
                    mongo.Disconnect();
                }
            }
        }

        /// <summary>
        /// 获取Mongo服务器信息
        /// </summary>
        /// <returns>
        /// </returns>
        private MongoServer GetMongo()
        {
            MongoUrl url = MongoUrl.Create(ConnectionString);
            return MongoServer.Create(url);
        }

        /// <summary>
        /// 构建Bson对象
        /// </summary>
        /// <param name="log">日志对象</param>
        /// <returns></returns>
        private BsonDocument BuildBsonDocument(LoggingEvent log)
        {
            return BackwardCompatibility.BuildBsonDocument(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void WriteLog(string info)
        {
            try
            {
                string m_LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"log" + Path.AltDirectorySeparatorChar);
                if (!Directory.Exists(m_LogDirPath))
                {
                    Directory.CreateDirectory(m_LogDirPath);
                }
                string fileName = string.Format("{0}{1}.log", m_LogDirPath, DateTime.Now.ToString("yyyyMMdd_HH"));
                using (StreamWriter file = new StreamWriter(File.Open(fileName, FileMode.Append)))
                {
                    file.WriteLine(info);
                    file.Flush();
                    file.Close();
                }
            }
            catch
            {
            }
        }
    }
}
