using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Entities
{
    public class DocType
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int idBioSignConfig;

        public int IdBioSignConfig
        {
            get { return idBioSignConfig; }
            set { idBioSignConfig = value; }
        }
        private String metadata;

        public String Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }
        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        private String reference;

        public String Reference
        {
            get { return reference; }
            set { reference = value; }
        }
        private int signerNumber;

        public int SignerNumber
        {
            get { return signerNumber; }
            set { signerNumber = value; }
        }
        private String signType;

        public String SignType
        {
            get { return signType; }
            set { signType = value; }
        }
    }
}
