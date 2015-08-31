using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.WebScoketServer;
using WacomWebSocketService.Control;

namespace WacomWebSocketService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if true
            
            //Test.SinWebSocket test = new Test.SinWebSocket();
            //test.getJsonOperation("20150827115225");
            //test.signPDF();
            Logic logic = Logic.getInstance();
            logic.onStart(); 

#else            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new DocumentRecieverService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
