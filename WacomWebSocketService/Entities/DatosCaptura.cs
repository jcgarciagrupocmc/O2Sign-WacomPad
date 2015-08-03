using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Entities
{
    public class DatosCaptura
    {
        public int uuid {get; set;}

        //public int Uuid
        //{
        //    get { return uuid; }
        //    set { uuid = value; }
        //}
        public String idoperation { get; set; }

        //public String Idoperation
        //{
        //    get { return idoperation; }
        //    set { idoperation = value; }
        //}
        public String value { get; set; }

        //public String Value
        //{
        //    get { return this.value; }
        //    set { this.value = value; }
        //}
        public String docname { get; set; }

        //public String Docname
        //{
        //    get { return docname; }
        //    set { docname = value; }
        //}
    }
}
