using System;
using System.Collections.Generic;

namespace WacomWebSocketService.Entities
{
    /**
     * @Class Bean that represents a Document to sign
     */
    public class DocumentData
    {
        //Document uuid
        private int uuid;
        //uuid property
        public int Uuid
        {
            get { return uuid; }
            set { uuid = value; }
        }
        //Document filename
        private String docname;
        //filename property
        public String Docname
        {
            get { return docname; }
            set { docname = value; }
        }
        //unsigned file path
        private String docpath;
        //docpath property
        public String Docpath
        {
            get { return docpath; }
            set { docpath = value; }
        }
        //Document metadata part1
        private List<Signer> docmetadata;
        //docmetadata property
        public List<Signer> Docmetadata
        {
            get { return docmetadata; }
            set { docmetadata = value; }
        }
        //Document metadata part2
        private String docmetadata2;
        //docmetadata2 property
        public String Docmetadata2
        {
            get { return docmetadata2; }
            set { docmetadata2 = value; }
        }
        //Document operation id
        private String idoperation;
        //idoperation property
        public String Idoperation
        {
            get { return idoperation; }
            set { idoperation = value; }
        }
        //signed file path
        private String docsignedpath;
        //docsignedpath property
        public String Docsignedpath
        {
            get { return docsignedpath; }
            set { docsignedpath = value; }
        }
        //x coordinate of the sign (default value 100)
        private int x=100;
        //x property
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        //y coordinate of the sign (default value 100)
        private int y=100;
        //y property
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        //page coordinate of the sign (default value 1)
        private int page=1;
        //page property
        public int Page
        {
            get { return page; }
            set { page = value; }
        }
        public bool documentHasBeenSigned()
        {
            foreach (Signer s in this.docmetadata)
            {
                if (s.Signed)
                    return true;
            }
            return false;
        }
    }
}
