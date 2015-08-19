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
                for (int i = 0; i < doc.Docmetadata.Count;i++)
                    logic.signPdf(doc,i);
            }
            logic.uploadSignedOperation(list);

            return jsonData;
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
