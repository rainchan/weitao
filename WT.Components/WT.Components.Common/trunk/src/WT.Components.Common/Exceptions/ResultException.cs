using System;

namespace WT.Components.Common.Exceptions
{
    public class ResultException:Exception
    {
        private int m_ret;
        private string m_message;
        public ResultException(int ret, string message)
            : base(message)
        {
            m_ret = ret;
            m_message = message;
        }

        public int Ret
        {
            get { return m_ret; }
        }

        public string Message 
        {
            get { return m_message; }
        }
    }
}
