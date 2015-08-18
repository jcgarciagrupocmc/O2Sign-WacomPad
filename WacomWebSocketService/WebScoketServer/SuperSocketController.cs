using System;
using SuperSocket.SocketBase;
using log4net;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Exceptions;
using SuperWebSocket;

namespace WacomWebSocketService.WebScoketServer
{
    /**
     * @Class class to control the WebSocket
     */
    public class SuperSocketController
    {
        private WebSocketServer appServer;
        private ILog Log;

        /**
         * @Method constructor that initializates the WebSocket on port send by parameter
         * @Parameter port where WebSocket will listen
         */
        public SuperSocketController(int port)
        {
            if (LogManager.GetCurrentLoggers().Length > 0)
                this.Log = LogManager.GetCurrentLoggers()[0];
            else
                this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            if (port == 0)
                port = 82;
            this.appServer = new WebSocketServer();
            if (!this.appServer.Setup(port))
            {
                Log.Error("Error while creating Socket on port " + port);
                this.appServer = null;
            }
            else
            {
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }

        }

        /**
         * @Method constructor that initializates the WebSocket on default port 82
         */
        public SuperSocketController()
        {

            int port;
            if (LogManager.GetCurrentLoggers().Length > 0)
                this.Log = LogManager.GetCurrentLoggers()[0];
            else
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
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(this.appServer_NewMessageReceived);
            }
        }

        /**
         * @Method Open WebSocket connection
         * @Return true if OK, false otherwise
         */
        public bool open()
        {
            if (this.appServer != null)
            {
                if (!this.appServer.Start())
                {
                    Log.Error("Error while opening Socket");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else 
            { 
                return false;
            }

        }
        
        /**
         * @Method Close the WebSocket connection
         * @Return true if OK, false otherwise
         */
        public bool close()
        {
            if (this.appServer != null)
            {
                this.appServer.Stop();
                return true;
            }
            else
            {
                this.Log.Error("Error while closing Socket");
                return false;
            }
        }

        /**
         * @Method
         * @Params
         * @Params
         */
        internal void appServer_NewMessageReceived(WebSocketSession session, String message)
        {
            try
            {
                session.Send(Control.Logic.getInstance().recieveWebSMessage(message));
            }
            catch (IncorrectMessageException ex)
            {
                Log.Error("IncorrectMessageException", ex);
                session.Send(ex.response);
            }
        }
        /**
         * @Method Create a new WebSocket Response of type Connection
         * @Return WebSocket Response Entity
         */
        public Response createRegisterResponse()
        {
            return new Response { Type = ResponseType.Connection };
        }
        /**
         * @Method Create a new WebSocket Response of type Error
         * @Params Response json message
         * @Return WebSocket Response Entity
         */
        public Response createErrorResponse(String s)
        {
            return new Response { Type = ResponseType.Error, Data = new { s } };
        }


        /**
         * @Method Create a new WebSocket Response of type Error
         * @Return WebSocket Response Entity
         */
        public Response createErrorResponse()
        {
            return new Response { Type = ResponseType.Error };
        }


        /**
         * @Method Create a new WebSocket Response of type OperationOK for UploadOperation Command
         * @Return WebSocket Response Entity
         */
        public Response createOperationOKResponse()
        {
            return new Response { Type = ResponseType.OperationOK};
        }
        /**
         * @Method Create a new WebSocket Response of type
         * 
         * @Return WebSocket Response Entity
         */
        public Response createOperationListResponse(String jsonObject)
        {
            return new Response { Type = ResponseType.OperationList, Data=jsonObject };
        }
        /**
         * @Method Create a new WebSocket Response of type File
         * 
         * @Return WebSocket Response Entity
         */
        public Response createFileResponse(String file)
        {
            return new Response { Type = ResponseType.File, Data = file };
        }
        /**
         * @Method Create a new WebSocket Response of type SignedFile
         * @Return WebSocket Response Entity
         */
        public Response createSignedFileResponse()
        {
            return new Response { Type = ResponseType.SignedFile };
        }
        /**
         * @Method Create a new WebSocket Response of type OperationCanceled for CancelOperation Command
         * @Return WebSocket Response Entity
         */
        public Response createOperationCanceledResponse()
        {
            return new Response { Type = ResponseType.OperationCanceled};
        }


    }
}
