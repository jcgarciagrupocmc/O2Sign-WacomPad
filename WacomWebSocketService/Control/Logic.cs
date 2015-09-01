using System;
using System.Collections.Generic;
using System.Linq;
using WacomWebSocketService.Entities;
using WacomWebSocketService.Consts;
using System.IO;
using log4net;
using log4net.Config;
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
            log4net.Config.XmlConfigurator.Configure();
            this.padController = new WacomPad.WacomPadController();
            this.pdfController = new PDF.PDFController();
            this.restController = new WSClient.RestController(Properties.Settings.Default.host);
            this.wsController = new WebScoketServer.SuperSocketController();
            this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
        }
        /**
         * @Method checks if WacomPad is conected and if so start the WebSocket and waits in active waiting loop
         */
        public void onStart()
        {

#if DEBUG
            if (padController.checkPadConnected())
            {
                this.wsController.open();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            } 
#else
            
            this.wsController.open();
            Log.Debug("System goes to sleep");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            //this.wsController.open();
#endif
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
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\" + list.ElementAt(0).Idoperation))
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation, true);
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation);
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation + "\\signed");

                //retrieving all the operation documents from REST WS
                foreach (DocumentData doc in list)
                {
                    String url = String.Format(WSMethods.GET_PDF_BY_ID, doc.Uuid);
                    Byte[] pdfBytes = this.restController.doGet(url);
                    if (pdfBytes != null)
                    {
                        doc.Docpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation + "\\" + doc.Docname;
                        doc.Docsignedpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation + "\\signed\\" + doc.Docname;
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
                
                List<Signer> signers = this.getRemainingSigners(doc);
                if (doc.documentHasBeenSigned())
                    return Parser.serializeObject(this.wsController.createFileResponse(ControlTools.fileToBase64(doc.Docsignedpath), Parser.serializeObject(signers)));
                else
                    return Parser.serializeObject(this.wsController.createFileResponse(ControlTools.fileToBase64(doc.Docpath), Parser.serializeObject(signers)));
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
                //List<Signer> signers = this.getRemainingSigners(doc);
                List<Signer> signers = doc.Docmetadata;
                return Parser.serializeObject(this.wsController.createFileResponse(ControlTools.fileToBase64(doc.Docsignedpath),Parser.serializeObject(signers)));
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
            doc.Docmetadata[signer].Signed = false;
            if (this.padController.checkPadConnected())
            {
                this.Log.Debug(String.Format("The document {0} from operation {1} is going to be signed by {2}", doc.Docname, doc.Idoperation, doc.Docmetadata[signer]));
                this.Log.Debug("Calling Wacom Pad Controller");
                GraphSign sign = this.padController.padSigning(doc.Docmetadata[signer]);
                if (sign != null)
                {
                    this.Log.Debug("Graph Sign retrieved correctly");
                    String jsonSign = Parser.serializeObject(sign.Points);
                    //this.Log.Debug("JSON string for sign "+jsonSign);
                    String[] signArray = this.getSignString(sign);
                    doc.Docmetadata[signer].Signed = this.pdfController.doSignature(doc, sign, signArray, doc.Docmetadata[signer]);
                }

            }
            else
                throw new Exception("pad not connected");
                
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
        internal bool uploadSignedOperation(List<DocumentData> list)
        {
            List<DatosCaptura> result = new List<DatosCaptura>();
            if (!this.fullSigned(list))
            {
                Log.Debug("Operation not fulled signed");
                return false;
            }
            foreach (DocumentData doc in list)
            {               
                result.Add(ControlTools.toDatosCaptura(doc));
            }
            
                restController.doPost(WSMethods.UPLOAD_ALL_SIGNED_PDF, result);
                this.deleteTempFiles(list);
            return true;
        }
        /**
         * @Method deletes the temp files stored in localmachine
         */
        private void deleteTempFiles(List<DocumentData> list)
        {
            Log.Debug("Deleting temp files");
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation))
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation, true);
            else
                Log.Debug("Nothing to delete");
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + list.ElementAt(0).Idoperation))
                Log.Debug("Files deleted");
            else
                Log.Error("Files not deleted");
        }
        /**
         * 
         */
        internal bool fullSigned(List<DocumentData> list)
        {
            foreach (DocumentData doc in list)
            {
                foreach (Signer s in doc.Docmetadata)
                {
                    if (!s.Signed)
                        return false;
                }
            }
            return true;
        }
        /**
         * 
         */
        internal string recieveWebSMessage(string message)
        {
            Log.Debug("Message Recieved from WebSocket: " + message);
            try
            {
                String url, jsonData;
                dynamic obj = Parser.deserializeObject(message);
                
                switch ((int)obj.Type)
                {
                    //Processing connection, handshake and register message from websocket
                    case (int)CommandType.Register:                        
                        if (padController.checkPadConnected())
                        {
                            return Parser.serializeObject(this.wsController.createRegisterResponse());
                        }
                        else
                        {
                            Log.Info("Wacom Pad not Connected");
                            return Parser.serializeObject(this.wsController.createErrorResponse());
                        }
                     //Processing getOperation message from websocket
                    case (int)CommandType.GetOperation:
                        if (obj.idFile != null)
                        {
                            url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, obj.idFile);
                            jsonData = this.restController.doGet(url, null);
                            if (jsonData != String.Empty)
                            {
                                Log.Debug("JSON Recieved from PIE: " + jsonData);
                                this.operation = this.newOperation(jsonData);
                                String s = Parser.serializeObject(operation);
                                return Parser.serializeObject(this.wsController.createOperationListResponse(jsonData,s));
                            }
                            else
                            {
                                Log.Error("Error no data recieved from PIE");
                                return Parser.serializeObject(this.wsController.createErrorResponse());
                            }
                                
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
                                return Parser.serializeObject(this.wsController.createRemainingSignersResponse());

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
                if(e.InnerException!=null)
                    throw new IncorrectMessageException(e, Parser.serializeObject(this.wsController.createErrorResponse(e.Message,e.InnerException.Message)));
                else
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

        /**
         * @Method get the Document Signers that have not yet signed
         * @Params Document to check Signers
         * @Return List of Signers
         */
        private List<Signer> getRemainingSigners(DocumentData doc)
        {
            List<Signer> result = new List<Signer>();
            foreach (Signer s in doc.Docmetadata)
                if (!s.Signed)
                    result.Add(s);
            return result;
        }

    }
}
