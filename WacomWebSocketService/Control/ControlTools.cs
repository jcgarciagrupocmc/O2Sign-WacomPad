using System;
using System.IO;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.Control
{
    /**
     * @Class Utils Class to support Logic
     */
    public class ControlTools
    {
        /**
         * @Method transforms a DocumentData into DatosCaptura reading the signed PDF File and encoding it in Base64
         * @Parameters document ready to send
         * @Return Encoded signed PDF File
         */
        public static DatosCaptura toDatosCaptura(DocumentData doc)
        {
            DatosCaptura result = new DatosCaptura();
            result.docname = doc.Docname;
            result.uuid = doc.Uuid;
            result.idoperation = doc.Idoperation;
            if (File.Exists(doc.Docsignedpath))
            {
                result.value = fileToBase64(doc.Docsignedpath);
            }
            else
                result.value = "";
            return result;
        }

        /**
         * @Method transforms the selected file to Base64 String
         * @Params file path
         * @Return Base64 string
         */
        public static String fileToBase64(String path)
        {
            FileStream fis = new FileStream(path, FileMode.Open, FileAccess.Read);
            Byte[] barray = new Byte[fis.Length];
            fis.Read(barray, 0, (int)fis.Length);
            fis.Close();
            return Convert.ToBase64String(barray);
        }
    }
}
