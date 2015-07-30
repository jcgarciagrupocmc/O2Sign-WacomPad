﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using log4net;
using WacomWebSocketService.JSONMessageConverter;
using Newtonsoft.Json;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Exceptions;
using SuperWebSocket;

namespace WacomWebSocketService.WebScoketServer
{
    public class SuperSocketController
    {
        private WebSocketServer appServer;
        private Control.Logic logic;

        /**
         * 
         * 
         */
        public SuperSocketController(Control.Logic pLogic, int port)
        {
            this.logic = pLogic;
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
         * 
         * 
         */
        public SuperSocketController(Control.Logic pLogic)
        {
            this.logic = pLogic;
            int port;
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
         * 
         * 
         * 
         */
        public bool open()
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
         * 
         * 
         * 
         */
        public bool close()
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (this.appServer != null)
            {
                this.appServer.Stop();
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * 
         * 
         */
        internal void appServer_NewMessageReceived(WebSocketSession session, String message)
        {
            try
            {
                //WebSocketMC  mc = new WebSocketMC();
                //session.Send(mc.recieveMessage(message));
                session.Send(this.logic.recieveWebSMessage(message));
            }
            catch (IncorrectMessageException ex)
            {
                session.Send(ex.response);
            }
        }
        /**
         * 
         */
        public Response createRegisterResponse()
        {
            return new Response { Type = ResponseType.Connection };
        }
        /**
         * 
         */
        public Response createErrorResponse(String s)
        {
            return new Response { Type = ResponseType.Error, Data = new { s } };
        }

        /**
         * 
         */
        public Response createErrorResponse()
        {
            return new Response { Type = ResponseType.Error };
        }

        /**
         * 
         */
        public Response createOperationOKResponse(String s)
        {
            return new Response { Type = ResponseType.DataRecieved, Data = s };
        }

    }
}