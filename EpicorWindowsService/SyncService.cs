using EpicorConsole.Services;
using System;
using System.ServiceProcess;

namespace EpicorWindowsService
{
    public partial class SyncService : ServiceBase
    {
        public SyncService()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();

            var partService = new PartService(sessionId);
            await partService.SyncParts();
        }

        protected override void OnStop()
        {
        }
    }
}
