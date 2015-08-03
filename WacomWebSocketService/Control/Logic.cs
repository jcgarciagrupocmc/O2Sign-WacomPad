using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WacomWebSocketService.Entities;
using WacomWebSocketService.Consts;
using System.IO;
using log4net;
using WacomWebSocketService.Exceptions;

namespace WacomWebSocketService.Control
{
    public class Logic
    {
        private WacomPad.WacomPadController padController;
        private PDF.PDFController pdfController;
        private WebScoketServer.SuperSocketController wsController;
        private WSClient.RestController restController;

        public WacomPad.WacomPadController getPadController()
        {
            return this.padController;
        }

        public PDF.PDFController getPdfController()
        {
            return this.pdfController;
        }

        public WSClient.RestController getRestController()
        {
            return this.restController;
        }

        public Logic()
        {
            this.padController = new WacomPad.WacomPadController();
            this.pdfController = new PDF.PDFController();
            this.restController = new WSClient.RestController(Properties.Settings.Default.host);
            this.wsController = new WebScoketServer.SuperSocketController(this);

        }
        /**
         * 
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
         * 
         */
        internal List<DocumentData> newOperation(string jsonOperation)
        {
            List<DocumentData> list = JsonConvert.DeserializeObject<List<DocumentData>>(jsonOperation);
            if ((list!=null) && (list.Count > 0)) { 
                if (Directory.Exists(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation))
                    Directory.Delete(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation, true);
                Directory.CreateDirectory(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation);
                Directory.CreateDirectory(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation + "\\signed");

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
                //TODO print list
            }

            return list;

            //throw new NotImplementedException();
        }
        
        /**
         * 
         */
        internal void showPdfUnsigned()
        {
        }
        /**
         * 
         */
        internal void showPdfSigned() 
        { 
        }
        /**
         * 
         */
        internal bool signPdf(DocumentData doc)
        {
            if(this.padController.checkPadConnected())
            {
                GraphSign sign = this.padController.padSigning();
                return this.pdfController.doSignature(doc, sign);
            }
            return false; 
        }

        internal bool uploadSignedOperation(List<DocumentData> list)
        {
            foreach (DocumentData doc in list)
            {
                //String jsonObject = JsonConvert.SerializeObject(ControlTools.toDatosCaptura(doc));
                restController.doPost(WSMethods.UPLOAD_SIGNED_PDF, ControlTools.toDatosCaptura(doc));
            }
            return true;
        }
        /**
         * 
         */
        internal string recieveWebSMessage(string message)
        {
            ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                String url, jsonData;
                dynamic obj = JsonConvert.DeserializeObject(message);
                switch ((int)obj.Type)
                {
                    case (int)CommandType.Register:
                        return JsonConvert.SerializeObject(this.wsController.createRegisterResponse());
                    case (int)CommandType.GetOperation:
                        if (obj.idFile != null)
                        {
                            url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, obj.idFile);
                            jsonData = this.restController.doGet(url, null);
                            this.newOperation(jsonData);
                            return JsonConvert.SerializeObject(this.wsController.createOperationOKResponse(jsonData));
                        }
                        else
                            return JsonConvert.SerializeObject(this.wsController.createErrorResponse());
                    case (int)CommandType.GetFile:
                        if (obj.idFile != null)
                        {
                            url = String.Format(WSMethods.GET_PDF_BY_ID, obj.idFile);
                            jsonData = this.restController.doGet(url, null);
                            return JsonConvert.SerializeObject(this.wsController.createOperationOKResponse(jsonData));
                        }
                        else
                            return JsonConvert.SerializeObject(this.wsController.createErrorResponse());
                            
                }
            }
            catch (System.Exception e) // Bad JSON! For shame.
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                throw new IncorrectMessageException(e, JsonConvert.SerializeObject(this.wsController.createErrorResponse(e.Message)));
            }
            throw new NotImplementedException();
        }


    }
}
