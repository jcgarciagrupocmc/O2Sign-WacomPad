using System;
using System.Collections.Generic;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Control;
using WacomWebSocketService.Entities;
using WacomWebSocketService.PDF;

namespace WacomWebSocketService.Test
{
    /**
     * @Class for testing
     */
    class SinWebSocket
    {
        /**
         * @Method recieves an idoperation and simulates all the signing process avoiding user interaction unless pad interaction and upload the files at the end
         * @Params operation id
         * @Return json operation object recieved from REST WS
         */
        public string getJsonOperation(string idOperation)
        {
            String url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, idOperation);
            Logic logic = Logic.getInstance();
            String jsonData = logic.getRestController().doGet(url, null);
            List<DocumentData> list = logic.newOperation(jsonData);
            foreach (DocumentData doc in list)
            {
                logic.signPdf(doc);
            }
            logic.uploadSignedOperation(list);

            return jsonData;
        }
        /**
         * @Method creates 2 documents from 2 known PDF files on disk and try to sign them
         */
        public void signPDF()
        {
            List<DocumentData> list = new List<DocumentData>();
            
            DocumentData doc = new DocumentData();
            doc.Idoperation = "1";
            doc.Docname = "pdf1.pdf";
            doc.Docpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\" + doc.Docname;
            doc.Docsignedpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\signed\\" + doc.Docname;
            //doc.X = 500;
            //doc.Y = 100;
            //doc.Page = 2;
            list.Add(doc);
            doc = new DocumentData();
            doc.Idoperation = "1";
            doc.Docname = "pdf5.pdf";
            doc.Docpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\" + doc.Docname;
            doc.Docsignedpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\signed\\" + doc.Docname;
            //doc.X = 100;
            //doc.Y = 400;
            //doc.Page = 1;
            list.Add(doc);
            Logic logic = Logic.getInstance();
            foreach (DocumentData d in list)
            {
                logic.signPdf(d);
            }
        }
        /**
         * @Method read certificate files from embebed resources and print them on console
         */
        public void readCert()
        {
            Console.Out.WriteLine(DigitalSignUtils.readResourceFile(Properties.Settings.Default.certFile));
            Console.Out.WriteLine("----------------------------------------------------------------------");
            Console.Out.WriteLine(DigitalSignUtils.readResourceFile(Properties.Settings.Default.keyFile));
        }
    }
}
