using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Consts;
using WacomWebSocketService.Control;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.Test
{
    class SinWebSocket
    {
        private Logic logic;

        public SinWebSocket()
        {
            this.logic = new Logic();
        }

        public SinWebSocket(Logic pLogic)
        {
            this.logic = pLogic;
        }

        public string getJsonOperation(string idOperation)
        {
            String url = String.Format(WSMethods.GET_DOCS_BY_OPERATION, idOperation);
            String jsonData = this.logic.getRestController().doGet(url, null);
            List<DocumentData> list = logic.newOperation(jsonData);
            foreach (DocumentData doc in list)
            {
                logic.signPdf(doc);
            }
            logic.uploadSignedOperation(list);

            return jsonData;
        }

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

            foreach (DocumentData d in list)
            {
                logic.signPdf(d);
            }
        }
    }
}
