using System;

namespace WacomWebSocketService.Entities
{
    public class DatosCaptura
    {
        //document uuid
        public int uuid {get; set;}

        //public int Uuid
        //{
        //    get { return uuid; }
        //    set { uuid = value; }
        //}

        //operation of the document
        public String idoperation { get; set; }

        //public String Idoperation
        //{
        //    get { return idoperation; }
        //    set { idoperation = value; }
        //}

        //PDF file on Base64
        public String value { get; set; }

        //public String Value
        //{
        //    get { return this.value; }
        //    set { this.value = value; }
        //}

        //filename
        public String docname { get; set; }

        //public String Docname
        //{
        //    get { return docname; }
        //    set { docname = value; }
        //}
    }
}
