using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Entities
{
    public class DocumentData
    {
        private int uuid;

        public int Uuid
        {
            get { return uuid; }
            set { uuid = value; }
        }
        private String docname;

        public String Docname
        {
            get { return docname; }
            set { docname = value; }
        }
        private String docpath;

        public String Docpath
        {
            get { return docpath; }
            set { docpath = value; }
        }
        private String docmetadata;

        public String Docmetadata
        {
            get { return docmetadata; }
            set { docmetadata = value; }
        }
        private String docmetadata2;

        public String Docmetadata2
        {
            get { return docmetadata2; }
            set { docmetadata2 = value; }
        }
        private String idoperation;

        public String Idoperation
        {
            get { return idoperation; }
            set { idoperation = value; }
        }

        private String docsignedpath;

        public String Docsignedpath
        {
            get { return docsignedpath; }
            set { docsignedpath = value; }
        }
    }
}
