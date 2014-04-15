using log4net.Core;
using log4net.Util;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WT.Components.Common.Log.Mongo
{
    public class BackwardCompatibility
    {
        private static List<string> recordProperties = new List<string> { "method", "category" };

        /// <summary>
        /// 将loggingEvent对象转换成Bson
        /// </summary>
        /// <param name="loggingEvent">
        /// loggingEvent对象
        /// </param>
        /// <returns></returns>
        public static BsonDocument BuildBsonDocument(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                return null;
            }

            var toReturn = new BsonDocument {
				{"timestamp", loggingEvent.TimeStamp}, 
				{"level", loggingEvent.Level.ToString()}, 
				//{"thread", loggingEvent.ThreadName}, 
				//{"userName", loggingEvent.UserName}, 
				{"message", loggingEvent.RenderedMessage}, 
				{"loggerName", loggingEvent.LoggerName}, 
				//{"domain", loggingEvent.Domain}, 
				{"machineName", Environment.MachineName}
			};

            // 记录Properties
            recordProperties.ForEach(name => 
            {
                if (loggingEvent.Properties.Contains(name))
                {
                    toReturn.Add(name, Convert.ToString(loggingEvent.Properties[name]));
                }
            });

            // location information, if available
            if (loggingEvent.LocationInformation != null)
            {
                toReturn.Add("fileName", loggingEvent.LocationInformation.FileName);

                // 前面没设置过方法
                if (!toReturn.Contains("method"))
                {
                    toReturn.Add("method", loggingEvent.LocationInformation.MethodName);
                }
                
                toReturn.Add("lineNumber", loggingEvent.LocationInformation.LineNumber);
                toReturn.Add("className", loggingEvent.LocationInformation.ClassName);
            }

            // exception information
            if (loggingEvent.ExceptionObject != null)
            {
                toReturn.Add("exception", BuildExceptionBsonDocument(loggingEvent.ExceptionObject));
            }

            // properties
            PropertiesDictionary compositeProperties = loggingEvent.GetProperties();
            if (compositeProperties != null && compositeProperties.Count > 0)
            {
                var properties = new BsonDocument();
                foreach (DictionaryEntry entry in compositeProperties)
                {
                    if (!toReturn.Contains(entry.Key.ToString()))
                    {
                        properties.Add(entry.Key.ToString(), entry.Value.ToString());
                    }
                }

                toReturn.Add("properties", properties);
            }

            return toReturn;
        }

        /// <summary>
        /// 将Exception转换为Bson
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        private static BsonDocument BuildExceptionBsonDocument(Exception ex)
        {
            var toReturn = new BsonDocument {
				{"message", ex.Message}, 
				{"source", ex.Source}, 
				{"stackTrace", ex.StackTrace}
			};

            if (ex.InnerException != null)
            {
                toReturn.Add("innerException", BuildExceptionBsonDocument(ex.InnerException));
            }

            return toReturn;
        }
    }
}
