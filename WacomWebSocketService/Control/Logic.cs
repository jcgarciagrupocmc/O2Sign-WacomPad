using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WacomWebSocketService.Entities;
using WacomWebSocketService.WSClient;
using WacomWebSocketService.Consts;
using System.IO;

namespace WacomWebSocketService.Control
{
    class Logic
    {
        
        internal void newOperation(string jsonOperation)
        {
            List<DocumentData> list = JsonConvert.DeserializeObject<List<DocumentData>>(jsonOperation);
            if (list.Count > 0) { 
                PieWSClient ws = new PieWSClient(Properties.Settings.Default.host);
                if (Directory.Exists(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation))
                    Directory.Delete(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation, true);
                Directory.CreateDirectory(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation);

                foreach (DocumentData doc in list)
                {
                    String url = String.Format(WSMethods.GET_PDF_BY_ID, doc.Uuid);
                    Byte[] pdfBytes = ws.doGet(url);
                    FileStream fis = File.Create(Properties.Settings.Default.tempPath + list.ElementAt(0).Idoperation + "\\" + doc.Docname, pdfBytes.Length);
                    fis.Write(pdfBytes, 0, pdfBytes.Length);
                    fis.Flush();
                    fis.Close();
                }
            }

            //throw new NotImplementedException();
        }
    }
}
