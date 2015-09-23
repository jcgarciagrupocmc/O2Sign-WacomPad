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
    public class GenericSSController
    {
        protected WebSocketServer appServer;
        protected ILog Log;

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
                    Log.Debug("socket opened");
                    return true;
                }
            }
            else
            {
                Log.Debug("Already exists a server");
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
                Log.Debug("Message Recieved " + message);
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
        public static Response createRegisterResponse()
        {
            return new Response { Type = ResponseType.Connection };
        }
        /**
         * @Method Create a new WebSocket Response of type Error
         * @Params Response json message
         * @Return WebSocket Response Entity
         */
        public static Response createErrorResponse(String s)
        {
            return new Response { Type = ResponseType.Error, Data = new { s } };
        }
        /**
         * @Method Create a new WebSocket Response of type Error
         * @Params Response json message
         * @Return WebSocket Response Entity
         */
        public static Response createErrorResponse(String s, String s2)
        {
            return new Response { Type = ResponseType.Error, Data = new { s }, Data2 = s2 };
        }
        /**
         * @Method Create a new WebSocket Response of type Error
         * @Return WebSocket Response Entity
         */
        public static Response createErrorResponse()
        {
            return new Response { Type = ResponseType.Error };
        }
        /**
         * @Method Create a new WebSocket Response of type OperationOK for UploadOperation Command
         * @Return WebSocket Response Entity
         */
        public static Response createOperationOKResponse()
        {
            return new Response { Type = ResponseType.OperationOK };
        }
        /**
         * @Method Create a new WebSocket Response of type
         * 
         * @Return WebSocket Response Entity
         */
        public static Response createOperationListResponse(String jsonObject)
        {
            return new Response { Type = ResponseType.OperationList, Data = jsonObject };
        }
        /**
         * @Method Create a new WebSocket Response of type
         * 
         * @Return WebSocket Response Entity
         */
        public static Response createOperationListResponse(String jsonObject, String operation)
        {
            return new Response { Type = ResponseType.OperationList, Data = jsonObject, Data2 = operation };
        }
        /**
         * @Method Create a new WebSocket Response of type File
         * 
         * @Return WebSocket Response Entity
         */
        public static Response createFileResponse(String file, String signers)
        {
            return new Response { Type = ResponseType.File, Data = file, Data2 = signers };
        }
        /**
         * @Method Create a new WebSocket Response of type SignedFile
         * @Return WebSocket Response Entity
         */
        public static Response createSignedFileResponse()
        {
            return new Response { Type = ResponseType.SignedFile };
        }
        /**
         * @Method Create a new WebSocket Response of type OperationCanceled for CancelOperation Command
         * @Return WebSocket Response Entity
         */
        public static Response createOperationCanceledResponse()
        {
            return new Response { Type = ResponseType.OperationCanceled };
        }
        /**
         * @Method Create a new WebSocket Response of type RemainingSigners for UploadOperation Command
         * @Return WebSocket Response Entity 
         */
        public static Response createRemainingSignersResponse()
        {
            return new Response { Type = ResponseType.RemainingSigners };
        }
    }
}
