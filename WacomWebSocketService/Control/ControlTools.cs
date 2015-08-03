using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.Control
{
    public class ControlTools
    {
        public static DatosCaptura toDatosCaptura(DocumentData doc)
        {
            DatosCaptura result = new DatosCaptura();
            result.docname = doc.Docname;
            result.uuid = doc.Uuid;
            result.idoperation = doc.Idoperation;
            if (File.Exists(doc.Docsignedpath))
            {
                FileStream fis = new FileStream(doc.Docsignedpath, FileMode.Open, FileAccess.Read);
                Byte[] barray = new Byte[fis.Length];
                fis.Read(barray, 0, (int)fis.Length);
                fis.Close();
                result.value = Convert.ToBase64String(barray);
            }
            else
                result.value = "";
            return result;
        }
    }
}
