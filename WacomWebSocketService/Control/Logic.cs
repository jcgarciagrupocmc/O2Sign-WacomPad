using System;
using System.Collections.Generic;
using System.Linq;
using WacomWebSocketService.Entities;
using WacomWebSocketService.Consts;
using System.IO;
using log4net;
using WacomWebSocketService.Exceptions;

namespace WacomWebSocketService.Control
{
    /**
     * @Class This Class implements the application business logic.
     * It has references to the controller classes for each interface in the app: WebSocket, REST Services, PDF module and Wacom Pad 
     */
    public class Logic
    {
        private WacomPad.WacomPadController padController;
        private PDF.PDFController pdfController;
        private WebScoketServer.SuperSocketController wsController;
        private WSClient.RestController restController;
        private static Logic instance;
        private ILog Log;
        private List<DocumentData> operation;


        /**
         * @Method Getter for the Wacom Pad Controller Class
         */
        public WacomPad.WacomPadController getPadController()
        {
            return this.padController;
        }

        /**
         * @Method Getter for the PDF Controller Class
         */
        public PDF.PDFController getPdfController()
        {
            return this.pdfController;
        }

        /**
         * @Method Getter for the REST WebServices Controller Class
         */
        public WSClient.RestController getRestController()
        {
            return this.restController;
        }

        /**
         * @Method Getter for the WebSocket Controller Class
         */
        public WebScoketServer.SuperSocketController getWebSocketController()
        {
            return this.wsController;
        }

        /**
         * @Method getInstance Method for singleton
         */
        public static Logic getInstance()
        {
            if (instance == null)
            {
                instance = new Logic();
            }
            return instance;
        }
        /**
         * @Constructor private constructor for singleton initializing all the controllers
         */
        private Logic()
        {
            this.padController = new WacomPad.WacomPadController();
            this.pdfController = new PDF.PDFController();
            this.restController = new WSClient.RestController(Properties.Settings.Default.host);
            this.wsController = new WebScoketServer.SuperSocketController();
            if (LogManager.GetCurrentLoggers().Length > 0)
                this.Log = LogManager.GetCurrentLoggers()[0];
            else
                this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);

        }
        /**
         * @Method checks if WacomPad is conected and if so start the WebSocket and waits in active waiting loop
         */
        public void onStart()
        {
            if (padController.checkPadConnected())
            {
                this.wsController.open();
                while (true)
                {
                    continue;
                }
            }
        }
        /**
         * @Method parses the newoperation json recieved from REST WS and gets all the operation documents saving them temporally on disk
         * @Params string jsonOperation json data recieved from REST WS
         */
        internal List<DocumentData> newOperation(string jsonOperation)
        {
            //Creating disk directory structure for the operation
            List<DocumentData> list = Parser.parseDocumentData(jsonOperation);
            //List<DocumentData> list = JsonConvert.DeserializeObject<List<DocumentData>>(jsonOperation);
            if ((list!=null) && (list.Count > 0)) { 
                if (Directory.Exists(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation))
                    Directory.Delete(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation, true);
                Directory.CreateDirectory(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation);
                Directory.CreateDirectory(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation + "\\signed");

                //retrieving all the operation documents from REST WS
                foreach (DocumentData doc in list)
                {
                    String url = String.Format(WSMethods.GET_PDF_BY_ID, doc.Uuid);
                    Byte[] pdfBytes = this.restController.doGet(url);
                    if (pdfBytes != null)
                    {
                        doc.Docpath = Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation + "\\" + doc.Docname;
                        doc.Docsignedpath = Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation + "\\signed\\" + doc.Docname;
                        FileStream fis = File.Create(doc.Docpath, pdfBytes.Length);
                        fis.Write(pdfBytes, 0, pdfBytes.Length);
                        fis.Flush();
                        fis.Close();
                    }
                    
                }
            }

            return list;
        }
        /**
         * @Method get the unsigned PDF and build the websocket Response
         * @Params uuid of PDF document
         * @Return PDF file Response jsonObject if OK, otherwise Error Response json Object
         */
        internal String getPdfUnsigned(String uuid)
        {
            DocumentData doc = this.getDocuemnt(uuid);
            if (doc != null)
            {
                return Parser.serializeObject(this.wsController.createFileResponse(ControlTools.fileToBase64(doc.Docpath)));
            }
            else
            {
                Log.Error("Missed document " + uuid);
                return Parser.serializeObject(this.wsController.createErrorResponse());
            }
        }

        /**
         * @Method get the signed PDF and build the websocket Response
         * @Params uuid of PDF document
         * @Return PDF file Response jsonObject if OK, otherwise Error Response json Object
         */
        internal String getPdfSigned(String uuid)
        {
            DocumentData doc = this.getDocuemnt(uuid);
            if (doc != null)
            {
                return Parser.serializeObject(this.wsController.createFileResponse(ControlTools.fileToBase64(doc.Docsignedpath)));
            }
            else
            {
                Log.Error("Missed document " + uuid);
                return Parser.serializeObject(this.wsController.createErrorResponse());
            }
        }
        /**
         * @Method Check if the Wacom Pad is connected, if so calls the pad controller for signing 
         * and after calls PDF controller for digital signing
         * @Params DocumentData doc document to be signed
         */
        internal bool signPdf(DocumentData doc, int signer)
        {
            if (this.padController.checkPadConnected())
            {
                GraphSign sign = this.padController.padSigning();
                String jsonSign = Parser.serializeObject(sign.Points);
                String[] signArray = this.getSignString(sign);
                doc.Docmetadata[signer].Signed = this.pdfController.doSignature(doc, sign, signArray, doc.Docmetadata[signer]);
            }
            else
                doc.Docmetadata[signer].Signed = false;
            return doc.Docmetadata[signer].Signed;


        }

        /**
         * 
         */
        private String[] getSignString(GraphSign sign)
        {
            String[] result = new String[sign.Points.Count];
            int i = 0;
            foreach (BioSignPoint point in sign.Points)
            {
                result[i] = "" + point.Order + "," + point.X + "," + point.Y + "," + point.Pressure + "," + point.Time.Ticks;
                i++;
            }
            return result;
        }
        /**
         * @Method Search document and call for signing
         * @Params document uuid
         * @Return Response json object of type SignedFile if OK, Error otherwise
         */
        private string signPdf(string uuid, int signer)
        {
            DocumentData doc = this.getDocuemnt(uuid);
            if (this.signPdf(doc,signer))
            {
                return Parser.serializeObject(this.wsController.createSignedFileResponse());
            }
            else
            {
                return Parser.serializeObject(this.wsController.createErrorResponse("Sign Cancelled by User"));
            }
        }
        /**
         * @Method calls the REST WebService controller to do a POST request 
         * to the service for uploadsigned pdf foreach pdf document in the operation
         * @Params List<DocumentData> list a list of each Operation DocumentData
         */
        //TODO: Pending for revision to make atomic operation
        internal bool uploadSignedOperation(List<DocumentData> list)
        {
            foreach (DocumentData doc in list)
            {
                restController.doPost(WSMethods.UPLOAD_SIGNED_PDF, ControlTools.toDatosCaptura(doc));
            }
            return true;
        }
        /**
         * 
         */
        internal string recieveWebSMessage(string message)
        {
            try
            {
                String url, jsonData;
                dynamic obj = Parser.deserializeObject(message);
                
                switch ((int)obj.Type)
                {
                    //Processing connection, handshake and register message from websocket
                    case (int)CommandType.Register:
                        return Parser.serializeObject(this.wsController.createRegisterResponse());
                     //Processing getOperation message from websocket
                    case (int)CommandType.GetOperation:
                        if (obj.idFile != null)
                        {
                            url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, obj.idFile);
                            jsonData = this.restController.doGet(url, null);
                            if (jsonData != String.Empty)
                            {
                                this.operation = this.newOperation(jsonData);
                                return Parser.serializeObject(this.wsController.createOperationListResponse(jsonData));
                            }
                            else
                                return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                        else
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                    //Processing getUnsignedFile message from websocket
                    case (int)CommandType.GetFile:
                        if (obj.idFile != null)
                        {                            
                                return this.getPdfUnsigned((String)obj.idFile);                                                                                                              
                        }
                        else
                        {
                            Log.Error("Incorrect Parameter");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }

                    //Processing getSignedFile message from websocket
                    case (int)CommandType.GetSignedFile:
                        if (obj.idFile != null)
                        {
                            return this.getPdfSigned((String)obj.idFile);

                        }
                        else
                        {
                            Log.Error("Incorrect Parameter");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                    //Processing SignFile message from websocket
                    case (int)CommandType.SignFile:
                        if ((obj.idFile != null)&&(obj.idSigner!=null))
                        {
                            return this.signPdf((String)obj.idFile, (int)obj.idSigner);

                        }
                        else
                        {
                            Log.Error("Incorrect Parameter");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                    case (int)CommandType.UploadOperation:
                        if (obj.idFile != null)
                        {
                            if (this.uploadSignedOperation(this.operation))
                                return Parser.serializeObject(this.wsController.createOperationOKResponse());
                            else
                                return Parser.serializeObject(this.wsController.createErrorResponse());

                        }
                        else
                        {
                            Log.Error("Incorrect Parameter");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                    case (int)CommandType.CancelOperation:
                        if (obj.idFile != null)
                        {
                            return Parser.serializeObject(this.wsController.createOperationOKResponse());
                        }
                        else
                        {
                            Log.Error("Incorrect Parameter");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                            
                }
            }
            catch (System.Exception e) // Bad JSON! For shame.
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                throw new IncorrectMessageException(e, Parser.serializeObject(this.wsController.createErrorResponse(e.Message)));
            }
            throw new NotImplementedException();
        }

        /**
         * @Method seach a document in the current operation
         * @Params document uuid to search
         * @Return document finded, otherwise null
         */
        private DocumentData getDocuemnt(String idfile)
        {
            DocumentData result = null;
            foreach(DocumentData doc in this.operation)
            {
                if(doc.Uuid.ToString().Equals(idfile)){
                    result = doc;
                    break;
                }
                    
            }
            return result;
        }


    }
}
