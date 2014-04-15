using log4net.Util;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using log4net.Core;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace WT.Components.Common.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogUtil
    {
        // private static CTB.Common.Framework.Log.ILogWriter writer = null;
        private static log4net.ILog logger = null;
        static LogUtil()
        {
            //writer = new Log.LogWriter();
            logger = log4net.LogManager.GetLogger(typeof(LogUtil));
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            Info(message, method: null, category: null);
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method">方法名</param>
        /// <param name="category">分类</param>
        public static void Info(string message, string method = null, string category = null)
        {
            //lock (writer)
            //{
            //    writer.WriteAccessLog(message);
            //}
            //logger.Info(message);

            LoggingEvent loggingEvent = 
                CreateLoggingEvent(Level.Info, message: message, method: method, category: category);

            Log(loggingEvent);  
        }

        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message)
        {
            Warning(message, method: null, category: null);
        }

        /// <summary>
        /// 记录警告，用于未处理异常的捕获
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method">方法名</param>
        /// <param name="category">分类</param>
        public static void Warning(string message, string method = null, string category = null)
        {
            //lock (writer)
            //{
            //    writer.WriteWarningLog(message);
            //}
            //logger.Warn(message);

            LoggingEvent loggingEvent = 
                CreateLoggingEvent(Level.Warn, message: message, method: method, category: category);

            Log(loggingEvent);            
        }

        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="method">方法名</param>
        /// <param name="category">分类</param>
        public static void Warning(Exception ex, string method = null, string category = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("cause page:" + HttpContext.Current.Request.Url.ToString());
            sb.AppendLine("error message:" + ex.Message);
            sb.AppendLine("error source:" + ex.Source);
            sb.AppendLine("stack trace:" + ex.StackTrace);
            //lock (writer)
            //{
            //    writer.WriteWarningLog(sb.ToString());
            //}
            Warning(sb.ToString(), method, category);
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="message"></param>
        public static void Error(object message)
        {
            Error(message, method: null, category: null);
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method">方法名</param>
        /// <param name="category">分类</param>
        public static void Error(object message, string method = null, string category = null)
        {
            //logger.Error(message);

            LoggingEvent loggingEvent =
                CreateLoggingEvent(Level.Error, message: message, method: method, category: category);

            Log(loggingEvent);
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="message">异常描述信息</param>
        /// <param name="exception">异常信息</param>
        /// <param name="method">方法名</param>
        /// <param name="category">分类</param>
        public static void Error(object message, Exception exception, string method = null, string category = null)
        {
            //logger.Error(message, exception);

            LoggingEvent loggingEvent =
                CreateLoggingEvent(Level.Error, message, exception, method, category);

            Log(loggingEvent);
        }


        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void InfoFormat(string format, params object[] args)
        {
            logger.Info(new SystemStringFormat(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// 记录信息，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="format">异常描述信息</param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public static void InfoFormat(Exception exception, string format, params object[] args)
        {
            logger.Info(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
        }



        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WarningFormat(string format, params object[] args)
        {
            logger.Warn(new SystemStringFormat(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// 记录警告，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="format">异常描述信息</param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public static void WarningFormat(Exception exception, string format, params object[] args)
        {
            logger.Warn(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
        }


        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="format">异常描述信息</param>
        /// <param name="args">参数</param>
        public static void ErrorFormat(string format, params object[] args)
        {
            logger.Error(new SystemStringFormat(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="format">异常描述信息</param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public static void ErrorFormat(string format, Exception exception, params object[] args)
        {
            logger.Error(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void ErrorFormat(Exception exception, string format, params object[] args)
        {
            ErrorFormat(format, exception, args);
        }


        #region EventLog

        /// <summary>
        /// 往名称为“CheTuoBang”的系统事件日志中记录日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="logSource">事件源</param>
        public static void EventLog(Exception ex, string logSource = null)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (String.IsNullOrWhiteSpace(logSource))
            {
                logSource = "CheTuoBang";
            }
            
            var logName = "CheTuoBang";

            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(logSource))
                {
                    System.Diagnostics.EventLog.CreateEventSource(logSource, logName);
                }

                var log = new System.Diagnostics.EventLog(logName);
                log.Source = logSource;
                log.WriteEntry(ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            catch (Exception e)
            {
                var newException = new Exception(e.Message, ex);
                Error(newException, "EventLog", "EventLog");
            }
        }

        #endregion

        private static LoggingEvent CreateLoggingEvent(Level level, object message = null, Exception exception = null, string method = null, string category = null)
        {
            LoggingEvent loggingEvent = 
                new LoggingEvent(typeof(LogUtil), logger.Logger.Repository, logger.Logger.Name, level, message, exception);

            if (!String.IsNullOrWhiteSpace(method))
            {
                loggingEvent.Properties["method"] = method;
            }

            if (!String.IsNullOrWhiteSpace(category))
            {
                loggingEvent.Properties["category"] = category;
            }

            return loggingEvent;
        }

        private static void Log(LoggingEvent loggingEvent)
        {
            logger.Logger.Log(loggingEvent);
        }
    }
}
