﻿using System;
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
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            Logic logic = Logic.getInstance();
            //Logic logic = new Logic();
            Test.SinWebSocket test = new Test.SinWebSocket();
            test.getJsonOperation("20150810113119");
            //test.signPDF();
            //logic.onStart();
            //SuperSocketController ssController = new SuperSocketController();
            //ssController.open();
            //while (true)
            //{
            //    continue;
            //}
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new DocumentRecieverService() 
            //};
            //ServiceBase.Run(ServicesToRun);
        }
    }
}
