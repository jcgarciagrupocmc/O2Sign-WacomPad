using System;
using SuperSocket.SocketBase;
using log4net;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Exceptions;
using SuperWebSocket;
using WacomWebSocketService.Entities;
using System.Collections.Generic;

namespace WacomWebSocketService.WebScoketServer
{
    /**
     * @Class class to control the WebSocket
     */
    public class SuperSocketController :GenericSSController
    {
        //private WebSocketServer appServer;
        //private ILog Log;

        /**
         * @Method constructor that initializates the WebSocket on port send by parameter
         * @Parameter port where WebSocket will listen
         */
        public SuperSocketController(int port)
        {
            this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            if (port == 0)
                port = 82;
            this.appServer = new WebSocketServer();
            //this.appServer.Certificate = 
            if (!this.appServer.Setup(port))
            {
                Log.Error("Error while creating Socket on port " + port);
                this.appServer = null;
            }
            else
            {
                Log.Debug("WebSocket Started at port " + port);
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }

        }

        /**
         * @Method constructor that initializates the WebSocket on default port 82
         */
        public SuperSocketController()
        {

            int port;
            this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            port = 82;
            this.appServer = new WebSocketServer();
            if (!this.appServer.Setup(port))
            {
                Log.Error("Error while creating Socket on port " + port);
                this.appServer = null;
            }
            else
            {
                Log.Debug("WebSocket Started at port "+port);
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }
        }

    }
}
