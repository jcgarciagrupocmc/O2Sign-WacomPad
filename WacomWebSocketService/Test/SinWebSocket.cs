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
            logic.newOperation(jsonData);
            return jsonData;
        }

        public void signPDF()
        {
            DocumentData doc = new DocumentData();
            doc.Idoperation = "1";
            doc.Docname = "pdf1 - copia.pdf";
            doc.Docpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\" + doc.Docname;
            doc.Docsignedpath = Properties.Settings.Default.tempPath + "\\" + doc.Idoperation + "\\signed\\" + doc.Docname;
            logic.signPdf(doc);
        }
    }
}
