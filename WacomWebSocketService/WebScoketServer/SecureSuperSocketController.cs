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
    public class SecureSuperSocketController : GenericSSController
    {

        /**
         * @Method constructor that initializates the Secure WebSocket on port send by parameter
         * @Parameter port where WebSocket will listen
         */
        public SecureSuperSocketController(int port)
        {
            this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            SuperSocket.SocketBase.Config.RootConfig r = new SuperSocket.SocketBase.Config.RootConfig();

            SuperSocket.SocketBase.Config.ServerConfig s = new SuperSocket.SocketBase.Config.ServerConfig();

            //Using 80 or 9999 port is working

            s.Name = "SuperWebSocket";
            s.Ip = "127.0.0.1";
            s.Port = port;
            s.Mode = SocketMode.Tcp;
            s.Security = "tls";

            SuperSocket.SocketBase.Config.CertificateConfig cert = new SuperSocket.SocketBase.Config.CertificateConfig();

            string pathToBaseDir = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            cert.FilePath = System.IO.Path.Combine(pathToBaseDir, "galeonssl.pfx");
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(cert.FilePath));
            cert.Password = "1234";

            s.Certificate = cert;

            SuperSocket.SocketEngine.SocketServerFactory f = new SuperSocket.SocketEngine.SocketServerFactory();

            base.appServer = new WebSocketServer();
            try
            {
                base.appServer.Setup(r, s, f);
                Log.Debug("Secure WebSocket Started at port " + port);
                base.appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }
            catch (Exception ex)
            {
                Log.Error("Error while creating Socket on port " + port,ex);                
                this.appServer = null;
            }
        }

        /**
         * @Method constructor that initializates the Secure WebSocket on default port 1443
         */
        public SecureSuperSocketController()
        {
            this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            int port = 1443;
            SuperSocket.SocketBase.Config.RootConfig r = new SuperSocket.SocketBase.Config.RootConfig();

            SuperSocket.SocketBase.Config.ServerConfig s = new SuperSocket.SocketBase.Config.ServerConfig();

            //Using 80 or 9999 port is working

            s.Name = "SuperWebSocket";
            s.Ip = "127.0.0.1";
            s.Port = port;
            s.Mode = SocketMode.Tcp;
            s.Security = "tls";

            SuperSocket.SocketBase.Config.CertificateConfig cert = new SuperSocket.SocketBase.Config.CertificateConfig();

            string pathToBaseDir = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            cert.FilePath = System.IO.Path.Combine(pathToBaseDir, "galeonssl.pfx");
            System.Diagnostics.Debug.Assert(System.IO.File.Exists(cert.FilePath));
            cert.Password = "1234";

            s.Certificate = cert;

            SuperSocket.SocketEngine.SocketServerFactory f = new SuperSocket.SocketEngine.SocketServerFactory();

            base.appServer = new WebSocketServer();
            try
            {
                base.appServer.Setup(r, s, f);
                Log.Debug("Secure WebSocket Started at port " + port);
                base.appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }
            catch (Exception ex)
            {
                Log.Error("Error while creating Socket on port " + port, ex);
                this.appServer = null;
            }
        }
    }

}
