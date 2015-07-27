using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Entities
{
    class DatosCaptura
    {
        private int uuid;

        public int Uuid
        {
            get { return uuid; }
            set { uuid = value; }
        }
        private String idoperation;

        public String Idoperation
        {
            get { return idoperation; }
            set { idoperation = value; }
        }
        private String value;

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        private String docname;

        public String Docname
        {
            get { return docname; }
            set { docname = value; }
        }
    }
}
