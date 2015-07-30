﻿using Newtonsoft.Json;
using System;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Exceptions;
using WacomWebSocketService.WebScoketServer;
using log4net;
using WacomWebSocketService.WSClient;
using System.Collections.Generic;
using RestSharp;
using WacomWebSocketService.Control;

namespace WacomWebSocketService.JSONMessageConverter
{
    class WebSocketMC : MessageConverter
    {

        internal override string recieveMessage(string message)
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                Response r;
                RestController ws;
                dynamic obj = JsonConvert.DeserializeObject(message);
                Console.Out.WriteLine(message);
                switch ((int)obj.Type)
                {
                    case (int)CommandType.Register:
                        r = new Response { Type = ResponseType.Connection };
                        return JsonConvert.SerializeObject(r);
                    case (int)CommandType.GetOperation:
                        ws = new RestController(Properties.Settings.Default.host);
                        if (obj.idFile != null)
                        {
                            String url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, obj.idFile);
                            String result = ws.doGet(url, null);
                            if (result != String.Empty)
                            {
                                Logic logic = new Logic();
                                logic.newOperation(result);
                            }
                                
                            Console.WriteLine(result);
                            r = new Response { Type = ResponseType.DataRecieved, Data = result };
                        }
                        else
                            r = new Response { Type = ResponseType.Error };
                        return JsonConvert.SerializeObject(r);

                    case (int)CommandType.GetFile:
                        ws = new RestController(Properties.Settings.Default.host);
                        if (obj.idFile != null)
                        {
                            String url = String.Format(WSMethods.GET_PDF_BY_ID, obj.idFile);
                            String result = ws.doGet(url, null);
                            Console.WriteLine(result);
                            r = new Response { Type = ResponseType.FileRecieved, Data = result };
                        }
                        else
                            r = new Response { Type = ResponseType.Error };
                        return JsonConvert.SerializeObject(r);
                        //BroadcastCertsList(session);
                        // ListaCerts( session);
                        break;
                    

                }
            }
            catch (System.Exception e) // Bad JSON! For shame.
            {
                Response r = new Response { Type = ResponseType.Error, Data = new { e.Message } };
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                throw new IncorrectMessageException(e,JsonConvert.SerializeObject(r));
            }
            throw new NotImplementedException();
        }
    }
}