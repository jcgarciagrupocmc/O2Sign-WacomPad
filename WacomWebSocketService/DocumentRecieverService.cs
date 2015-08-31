using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Control;

namespace WacomWebSocketService
{
    public partial class DocumentRecieverService : ServiceBase
    {
        public DocumentRecieverService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logic logic = Logic.getInstance();
            logic.onStart();
            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        protected override void OnStop()
        {
        }
    }
}
