using System.ServiceProcess;

namespace WT.Services.AccessToken.WinService
{
    public partial class ServiceMain : ServiceBase
    {
        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceProcess.Current.Start();
        }

        protected override void OnStop()
        {
            ServiceProcess.Current.Stop();
        }
    }
}
