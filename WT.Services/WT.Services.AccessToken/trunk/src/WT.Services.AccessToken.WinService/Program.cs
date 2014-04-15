using System.ServiceProcess;

namespace WT.Services.AccessToken.WinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //ServiceProcess.Current.Start();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceMain() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
