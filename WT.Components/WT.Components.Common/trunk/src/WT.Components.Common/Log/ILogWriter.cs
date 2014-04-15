namespace WT.Components.Common.Log
{
    interface ILogWriter
    {
        void WriteAccessLog(string causer, string msg);
        void WriteAccessLog(string msg);
        void WriteErrLog(string causer, string msg);
        void WriteErrLog(string msg);
        void WriteWarningLog(string causer, string msg);
        void WriteWarningLog(string msg);
    }
}
