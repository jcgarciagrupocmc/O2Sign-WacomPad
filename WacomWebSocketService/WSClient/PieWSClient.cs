using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RestSharp;
using log4net;

namespace WacomWebSocketService.WSClient
{

    /**
     * This class implements generic calls to RESTful services using RestSharp Library
     */
    class PieWSClient
    {
        private RestClient client;
        /**
         * @METHOD this constructur inicializes the client
         * @PARAMS recieves the host to connect
         * Sets the encoding to UTF-8
         */ 
        public PieWSClient(String host)
        {
            this.client = new RestClient(host);
            this.client.Encoding = Encoding.UTF8;
        }

        /**
         * @METHOD this constructur inicializes the client
         * @PARAMS recieves the host to connect and the encoding type
         */
        public PieWSClient(String host, Encoding encoding)
        {
            this.client = new RestClient(host);
            this.client.Encoding = encoding;
        }

        //public String doPOST(String url, String jsonContent)
        //{
        //    HttpWebRequest POSTRequest = (HttpWebRequest)WebRequest.Create(url);
        //    POSTRequest.Method = "POST";
        //    POSTRequest.MediaType = "application/json";
        //    POSTRequest.TransferEncoding = "UTF-8";
        //    POSTRequest.KeepAlive = false;
        //    POSTRequest.Timeout = 5000;
        //    POSTRequest.ContentLength = jsonContent.Length;
        //    StreamWriter POSTStream = new StreamWriter(POSTRequest.GetRequestStream(), System.Text.Encoding.UTF8);
        //    POSTStream.Write(jsonContent);
        //    POSTStream.Close();
        //    try
        //    {
        //        WebResponse POSTResponse = POSTRequest.GetResponse();
        //        Stream webStream = POSTResponse.GetResponseStream();
        //        StreamReader responseReader = new StreamReader(webStream);
        //        string response = responseReader.ReadToEnd();
        //        Console.Out.WriteLine(response);
        //        responseReader.Close();
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Out.WriteLine("-----------------");
        //        Console.Out.WriteLine(e.Message);
        //        return "";
        //    }

        //    throw new NotImplementedException();

        //}


        //public String doGET(String url, String jsonContent){
        //    HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        //    GETRequest.Method = "GET";
        //    GETRequest.MediaType = "application/json";
        //    GETRequest.TransferEncoding ="UTF-8";
        //    GETRequest.KeepAlive= false;
        //    GETRequest.Timeout = 5000;
        //    GETRequest.ContentLength = jsonContent.Length;
        //    StreamWriter GETStream = new StreamWriter(GETRequest.GetRequestStream(), System.Text.Encoding.UTF8);
        //    GETStream.Write(jsonContent);
        //    GETStream.Close();
        //    try
        //    {
        //        WebResponse GETResponse = GETRequest.GetResponse();
        //        Stream webStream = GETResponse.GetResponseStream();
        //        StreamReader responseReader = new StreamReader(webStream);
        //        string response = responseReader.ReadToEnd();
        //        Console.Out.WriteLine(response);
        //        responseReader.Close();
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Out.WriteLine("-----------------");
        //        Console.Out.WriteLine(e.Message);
        //        return "";
        //    }


        //    throw new NotImplementedException();
        //}


        /**
         * @METHOD doGET implements a RESTful WS GET call.
         * 
         * @PARAMS url is the relative PATH of WS to call, parameters is the list of WS parameters
         * 
         * @RETURN the return for the WS call in json format.
         */
        public String doGet(String url, List<Parameter> parameters)
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            RestRequest GETRequest = new RestRequest(url, Method.GET);
            //GETRequest.Timeout = 5000;
            if(parameters!=null)
                foreach (Parameter p in parameters)
                    GETRequest.AddParameter(p);
            RestResponse GETResponse = (RestResponse)client.Execute(GETRequest);
            if(GETResponse.ResponseStatus== ResponseStatus.Completed)
                switch (GETResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return GETResponse.Content;
                    case HttpStatusCode.NoContent:
                        return "";
                    default:
                        break;
                  }
            else
                Log.Error("WebService Connection to " + url + " has ended with ResponseStatus " + GETResponse.ResponseStatus);
            return "";
        }
        /**
        * @METHOD doPOST implements a RESTful WS POST call.
        * 
        * @PARAMS url is the relative PATH of WS to call, jsonContent is the call json body
        * 
        * @RETURN the return for the WS call.
        */
        public String doPost(String url, String jsonContent)
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            RestRequest POSTRequest = new RestRequest(url, Method.POST);
            POSTRequest.Timeout = 5000;
            POSTRequest.AddJsonBody(jsonContent);
            RestResponse POSTResponse = (RestResponse)client.Execute(POSTRequest);
            if (POSTResponse.ResponseStatus == ResponseStatus.Completed)
                switch (POSTResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return POSTResponse.Content;
                    case HttpStatusCode.NoContent:
                        return "";
                    default:
                        break;
                }
            else
                Log.Error("WebService Connection to " + url + " has ended with ResponseStatus " + POSTResponse.ResponseStatus);
            return "";
            
        }

        /**
         * @METHOD doGET implements a RESTful WS GET call.
         * 
         * @PARAMS url is the relative PATH of WS to call
         * 
         * @RETURN the return for the WS call in Raw Byte Array.
         */
        public Byte[] doGet(String url)
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            RestRequest GETRequest = new RestRequest(url, Method.GET);
            GETRequest.AddHeader("Accept","application/pdf");
            //GETRequest.Timeout = 5000;
            RestResponse GETResponse = (RestResponse)client.Execute(GETRequest);
            if (GETResponse.ResponseStatus == ResponseStatus.Completed)
                switch (GETResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return GETResponse.RawBytes;
                    case HttpStatusCode.NoContent:
                        return null;
                    default:
                        break;
                }
            else
                Log.Error("WebService Connection to " + url + " has ended with ResponseStatus " + GETResponse.ResponseStatus);
            return null;
        }
    }
}
