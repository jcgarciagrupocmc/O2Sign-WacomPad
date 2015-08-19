using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Entities;
using Newtonsoft.Json;

namespace WacomWebSocketService.Control
{
    public class Parser
    {
        public static List<DocumentData> parseDocumentData(string jsonData)
        {
            List<DocumentData> result = new List<DocumentData>();
            dynamic data = JsonConvert.DeserializeObject(jsonData);
            foreach (dynamic obj in data) 
            {
                DocumentData doc = new DocumentData();
                doc.Docname = obj.docname;
                doc.Docmetadata2 = obj.docmetadata2.ToString();
                doc.Idoperation = obj.idoperation.ToString();
                doc.Uuid = (int)obj.uuid;
                doc.Docmetadata = parseSigners(obj.docmetadata.ToString());
                result.Add(doc);
            }

            return result;

        }
        public static List<Signer> parseSigners(string jsonData)
        {
            List<Signer> result = new List<Signer>();
            dynamic data = JsonConvert.DeserializeObject(jsonData);
            foreach (dynamic obj in data)
            {
                Signer signer = new Signer();
                signer.Nif = obj.nif;
                signer.Nombre = obj.nombre;
                signer.Page = obj.page;
                signer.Telefono = obj.telefono;
                signer.X = obj.x;
                signer.Y = obj.y;
                result.Add(signer);
            }

            return result;
        }

        public static String serializeObject(object r)
        {
            return JsonConvert.SerializeObject(r);
        }

        public static Object deserializeObject(String s)
        {
            return JsonConvert.DeserializeObject(s);
        }
    }
}
