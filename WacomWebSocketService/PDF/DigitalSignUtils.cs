using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;
using System.Security.Util;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.OpenSsl;
using System.IO;

namespace WacomWebSocketService.PDF
{
    class DigitalSignUtils
    {

        /**
         * This Method recieves a .cert file and read the certificate on it
         */
        public static X509Certificate readPublicKeyPem(string path)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                PemReader pem = new PemReader(reader);
                X509Certificate x509 = (X509Certificate)pem.ReadObject();
                reader.Close();
                fs.Close();
                return x509;
            }
            return null;
            
        }

        //public static PrivateKey readPrivateKeyPem(string path)
        //{

        //}

        public static Byte[] encrypt(Byte[] inpBytes, X509Certificate cert, String xform)
        {

            throw new NotImplementedException();
        }

    //    public static void signPDF(DocumentData doc)
    //    {

    //    }
    }
}
