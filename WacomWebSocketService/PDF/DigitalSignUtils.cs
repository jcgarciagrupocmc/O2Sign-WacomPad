using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using WacomWebSocketService.Entities;
using log4net;

namespace WacomWebSocketService.PDF
{
    class DigitalSignUtils
    {
        //static String certificadoPem = "-----BEGIN X509 CERTIFICATE-----\n" +
        //"MIIHpDCCBoygAwIBAgIJALuqnw6CQXyQMA0GCSqGSIb3DQEBBQUAMIHgMQswCQYD\n" +
        //"VQQGEwJFUzEuMCwGCSqGSIb3DQEJARYfYWNfY2FtZXJmaXJtYV9jY0BjYW1lcmZp\n" +
        //"cm1hLmNvbTFDMEEGA1UEBxM6TWFkcmlkIChzZWUgY3VycmVudCBhZGRyZXNzIGF0\n" +
        //"IHd3dy5jYW1lcmZpcm1hLmNvbS9hZGRyZXNzKTESMBAGA1UEBRMJQTgyNzQzMjg3\n" +
        //"MRkwFwYDVQQKExBBQyBDYW1lcmZpcm1hIFNBMS0wKwYDVQQDEyRBQyBDYW1lcmZp\n" +
        //"cm1hIENlcnRpZmljYWRvcyBDYW1lcmFsZXMwHhcNMTQwNjExMTEyMjMzWhcNMTYw\n" +
        //"NjEwMTEyMjMzWjCCAVIxLjAsBgNVBA0MJVF1YWxpZmllZCBDZXJ0aWZpY2F0ZTog\n" +
        //"Q0FNLVBKLVNXLUtQU0MxLzAtBgNVBAMMJkNNQyBPUEVSQVRJT05TIE9VVFNPVVJD\n" +
        //"SU5HIENNQ08yLCBTLkwuMSUwIwYJKoZIhvcNAQkBFhZqbGN1bXBsaWRvQGdydXBv\n" +
        //"Y21jLmVzMRIwEAYDVQQFEwlCODYyNzk4MTcxGTAXBgNVBAQMEEhPUlRFTEFOTyBM\n" +
        //"w5NQRVoxDjAMBgNVBCoMBUpBSU1FMRkwFwYKKwYBBAGBkxYBAQwJMTIzNjI1OTFF\n" +
        //"MRswGQYDVQQMDBJDT05TRUpFUk8gREVMRUdBRE8xEzARBgNVBAsMCkRJUkVDQ0nD\n" +
        //"k04xLzAtBgNVBAoMJkNNQyBPUEVSQVRJT05TIE9VVFNPVVJDSU5HIENNQ08yLCBT\n" +
        //"LkwuMQswCQYDVQQGEwJFUzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB\n" +
        //"ALyCuVJsSJCFZwsaYYVFeo5hn36cQzlSCXMePR8aBZKsUF4XlybwUOjRf5dNjjy9\n" +
        //"2TlITyg+wFs4cwWA6GMXzNkl/dPzAdZXzfKljK/ls+s8TdWgOuSxccEXJlPd8QoF\n" +
        //"3N7pwoFGY2x0+n/Q0RqWSQq+OKL3I9qa6U9rEnU0g7jqDvNlw9SwswhshHn+9pCt\n" +
        //"1hAcBnymCVtq+Y73W0mM3Xf4dXp1e815pA2oxfRCU9hqs7XPdD3nZMb7K9+bdJCH\n" +
        //"fUtqqvyO+O0vA6xS3FCJqLDWAbJNeavJoPB8BLbR7b5nIAioaM62E5iGaWlL8hWD\n" +
        //"bsc+z6lGgRuwc5f6J7GjFNcCAwEAAaOCAuowggLmMAwGA1UdEwEB/wQCMAAwEQYJ\n" +
        //"YIZIAYb4QgEBBAQDAgWgMA4GA1UdDwEB/wQEAwID+DAdBgNVHSUEFjAUBggrBgEF\n" +
        //"BQcDAgYIKwYBBQUHAwQwHQYDVR0OBBYEFPv/Rn7V3I8+UY32vVkFlN0G+xWbMHgG\n" +
        //"CCsGAQUFBwEBBGwwajBABggrBgEFBQcwAoY0aHR0cDovL3d3dy5jYW1lcmZpcm1h\n" +
        //"LmNvbS9jZXJ0cy9hY19jYW1lcmZpcm1hX2NjLmNydDAmBggrBgEFBQcwAYYaaHR0\n" +
        //"cDovL29jc3AuY2FtZXJmaXJtYS5jb20wgasGA1UdIwSBozCBoIAUth9OnRxokS43\n" +
        //"cmDhRo9apSoxMbmhgYSkgYEwfzELMAkGA1UEBhMCRVUxJzAlBgNVBAoTHkFDIENh\n" +
        //"bWVyZmlybWEgU0EgQ0lGIEE4Mjc0MzI4NzEjMCEGA1UECxMaaHR0cDovL3d3dy5j\n" +
        //"aGFtYmVyc2lnbi5vcmcxIjAgBgNVBAMTGUNoYW1iZXJzIG9mIENvbW1lcmNlIFJv\n" +
        //"b3SCAQUwdgYDVR0fBG8wbTA0oDKgMIYuaHR0cDovL2NybC5jYW1lcmZpcm1hLmNv\n" +
        //"bS9hY19jYW1lcmZpcm1hX2NjLmNybDA1oDOgMYYvaHR0cDovL2NybDEuY2FtZXJm\n" +
        //"aXJtYS5jb20vYWNfY2FtZXJmaXJtYV9jYy5jcmwwIQYDVR0RBBowGIEWamxjdW1w\n" +
        //"bGlkb0BncnVwb2NtYy5lczAqBgNVHRIEIzAhgR9hY19jYW1lcmZpcm1hX2NjQGNh\n" +
        //"bWVyZmlybWEuY29tMFUGA1UdIAROMEwwSgYNKwYBBAGBhy4KCQQBATA5MCkGCCsG\n" +
        //"AQUFBwIBFh1odHRwczovL3BvbGljeS5jYW1lcmZpcm1hLmNvbTAMBggrBgEFBQcC\n" +
        //"AjAAMC8GCCsGAQUFBwEDBCMwITAIBgYEAI5GAQEwFQYGBACORgECMAsTA0VVUgIB\n" +
        //"AAIBATANBgkqhkiG9w0BAQUFAAOCAQEAeEBpgh7Yh9zXSJNnEamscxgqYU6jwOdn\n" +
        //"toi9oIkxl5LJd/GKH8jY4k1nm4HkGLjEj3XKRFe/gFicW4U4l977EeEzxQRz1Nk8\n" +
        //"ovAceTvRAfjtxGXMmmwNoTLpnwekQiXXoBZc4Y8iZXEIuhCdVClSGj9sdfT+fuln\n" +
        //"b0Ct8Qk5DKE1OTHUruiP+CHE+L3AYzdAG6NwoMbIatYvJL/muYSLLCjmJNB6/xlj\n" +
        //"cH48OalrtFwm4TunLJYGLsUdk6XoOBwuxEqgjks/79ekyDpJcqnM3Hb4raNQn+UF\n" +
        //"/liD5wOSw1sq90Uud00a+01PgMEIkBkPaNWUb0PzXs+LU51nOs1Rtg==\n" +
        //"-----END X509 CERTIFICATE-----";
        //static String clavePrivadaPem = "-----BEGIN RSA PRIVATE KEY-----\n" +
        //"MIIEpAIBAAKCAQEAvIK5UmxIkIVnCxphhUV6jmGffpxDOVIJcx49HxoFkqxQXheX\n" +
        //"JvBQ6NF/l02OPL3ZOUhPKD7AWzhzBYDoYxfM2SX90/MB1lfN8qWMr+Wz6zxN1aA6\n" +
        //"5LFxwRcmU93xCgXc3unCgUZjbHT6f9DRGpZJCr44ovcj2prpT2sSdTSDuOoO82XD\n" +
        //"1LCzCGyEef72kK3WEBwGfKYJW2r5jvdbSYzdd/h1enV7zXmkDajF9EJT2Gqztc90\n" +
        //"Pedkxvsr35t0kId9S2qq/I747S8DrFLcUImosNYBsk15q8mg8HwEttHtvmcgCKho\n" +
        //"zrYTmIZpaUvyFYNuxz7PqUaBG7Bzl/onsaMU1wIDAQABAoIBABjJ7+jAyIIIqNYy\n" +
        //"7+QAkO8sMzcJAWYGPavfrBXs7BKvihn1bDD4pQsYkXiqACxIosn/kjkul0jnkWiF\n" +
        //"3Qk61fswHcLDT9iJz1E7J9bxk8k5MtsGqPftxZGoo6efpdS1lKfTgXpScTyP3Wj1\n" +
        //"YqnmXVCGVxhG+3YBrPB+mfzDnRaxpohFEHMAY7ZcPNVlh8FJ8jeo+p2wrLyG3IwC\n" +
        //"4yhjYVn2URERGU2mrZ5l9JcjCBC1WWEtALV9m/w2dU+Zx6icr/OqqU4DeP2jm0sk\n" +
        //"eWmKp/nW1HbUDPO3joaxbuKirNfe29ahrL4BG+GpT9BxX0fHRQQv+KaIKRkeavbO\n" +
        //"zOudMMECgYEA9Wkw/oh91gfCsw3EXOjr27df0+UwmA0AJf1kmvH0xnAJN1cA2w+v\n" +
        //"JeXIt9f6odln1SmX0idq2f+WCM496Gh7GCWNxsdMeGseas8jQrfk70QoT50m8Mg+\n" +
        //"t7zze5nODDpesJKeQy4Ahbpg7t0dFR+5jXQfmsTV9SyypSqvTzNxWScCgYEAxKUD\n" +
        //"JDvV+8FGF3qJOb4LqWG64eRqngG68HnENKFOfPCJYewFSBQdc5ut0ue2hQez2cd/\n" +
        //"Z43F/TV4OM9yXPkLXk4f/xnZMlml3ju3Ch6NglYUHEJn88Z5yyHUiME9Kwy+1Zpa\n" +
        //"m4H/lUVtBknYfQmOU8P4+xIpYj+vcP9FY1Kz1NECgYEA3YkE8nESb1h4GRzPazPU\n" +
        //"XnaN6gGWOVxbCvBQllLfLRdMKom+uwHQkXx6EgPFO7+/LL0sUUjF+17u+Kn9VsSi\n" +
        //"giy8bHnS/U1tmuu8H+lTn+4+GYh685dAjqrifWxdhcpXWjww4+IuFIooINi3/S6z\n" +
        //"WQ8/zH3tyzJ7XgBHW07L/rcCgYAVp5pF9jTlSW5fjYXpDu2X1IRQ9edryQL+Elqx\n" +
        //"9QWAqrhmSSh3vIdVwNIOhKfL1IJQvDBihfFEpgu8LdQJOv+ufen2HLGXYtnqNCc+\n" +
        //"/QhWTwZd6k4qQTQOU4ZlHOqQgBHP0fSiZVlw5blQ1Pb7Jf8/aDhV5bUa9aprRiEt\n" +
        //"A+F70QKBgQDgg/v5HK5KW/YC6iPTKl6UnuwtLQZcWRLqyKwFrKwlsTZCAiCu4jgz\n" +
        //"Vo8JbF+29ExgJhGFHpzkCNCI4v2PpJ7xJmIY2MR0Re6xm7sln2qnDYO2Y6YXHbjG\n" +
        //"sAUogU+WJM1ZWdHA1CoLJY96+ozhLVSZsQBVVSgK02N0JVt0emPymw==\n" +
        //"-----END RSA PRIVATE KEY-----\n";

        /**
         * This Method recieves a .cert file and read the certificate on it
         */
        //public static X509Certificate readPublicKeyPem(string path)
        //{
        //    if (File.Exists(path))
        //    {
        //        FileStream fs = new FileStream(path, FileMode.Open);
        //        StreamReader reader = new StreamReader(fs);
        //        PemReader pem = new PemReader(reader);
        //        X509Certificate x509 = (X509Certificate)pem.ReadObject();
        //        reader.Close();
        //        fs.Close();
        //        return x509;
        //    }
        //    return null;
            
        //}

        public static AsymmetricKeyParameter readPublicKeyPem(string pemFilename)
        {
            var fileStream = System.IO.File.OpenText(pemFilename);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(fileStream);
            var KeyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)pemReader.ReadObject();
            return KeyParameter;
        }

        public static AsymmetricKeyParameter readPrivateKey(string privateKeyFileName)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(privateKeyFileName))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            return keyPair.Private;
        }

        public static AsymmetricKeyParameter readPrivateKey()
        {
            AsymmetricCipherKeyPair keyPair;
            String pkString = readResourceFile(Properties.Settings.Default.keyFile);
            Byte[] ba = new Byte[pkString.Length];
            int i = 0;
            foreach (Char c in pkString.ToCharArray())
            {
                ba[i] = (Byte)c;
                i++;
            }
            MemoryStream ms = new MemoryStream(ba, true); 
            //StreamReader sr = new StreamReader(ms);
            using (StreamReader sr = new StreamReader(ms))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(sr).ReadObject();

            return keyPair.Private;
        }

        /**
         * @Method
         * @Return
         */
        public static Org.BouncyCastle.X509.X509Certificate[] crearCertificado()
        {
            ILog Log;
            if (LogManager.GetCurrentLoggers().Length > 0)
                Log = LogManager.GetCurrentLoggers()[0];
            else
                Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            //String parDeClaves = certificadoPem + "\n" + clavePrivadaPem;
            //Byte[] cert,key;
            MemoryStream ms = readCertFiles();
            //MemoryStream ms = new MemoryStream();         
           // ms.Write(resultCert.ToArray<Byte>(), 0, resultCert.Count());
            StreamReader sr = new StreamReader(ms);
            PemReader pem = new PemReader(sr);

            try
            {
                Org.BouncyCastle.X509.X509Certificate x509 = (Org.BouncyCastle.X509.X509Certificate)pem.ReadObject();
                Org.BouncyCastle.X509.X509Certificate[] chain = { x509 };
                sr.Close();
                ms.Close();
                return chain;

            }
            catch (IOException e)
            {
                Log.Error(e.Message, e);
                sr.Close();
                ms.Close();
                // TODO Auto-generated catch block
                
            }
            return null;
        }
        /**
         * 
         * 
         */
        private static MemoryStream readCertFiles()
        {
            String parDeClaves = readResourceFile(Properties.Settings.Default.certFile) + "\n" + readResourceFile(Properties.Settings.Default.keyFile);
            Byte[] ba = new Byte[parDeClaves.Length];
            int i = 0;
            foreach (Char c in parDeClaves.ToCharArray())
            {
                ba[i] = (Byte)c;
                i++;
            }
            MemoryStream ms = new MemoryStream(ba,true);
            return ms;            
        }

        //public static PrivateKey readPrivateKeyPem(string path)
        //{

        //}
        /**
         * TODO: not needed by now
         */
        public static Byte[] encrypt(Byte[] inpBytes, X509Certificate cert, String xform)
        {

            throw new NotImplementedException();
        }

        /**
         * 
         * 
         */
        public static void signPDF(DocumentData doc)
        {
            //BouncyCastleProvider provider = new BouncyCastleProvider();

            //  AddProvider(provider);
            ILog Log;
            if (LogManager.GetCurrentLoggers().Length > 0)
                Log = LogManager.GetCurrentLoggers()[0];
            else
                Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            try
            {
                PdfReader reader = new PdfReader(doc.Docsignedpath);
                if (File.Exists(doc.Docsignedpath + "-signed.pdf"))
                    File.Delete(doc.Docsignedpath + "-signed.pdf");
                FileStream fos = new FileStream(doc.Docsignedpath + "-signed.pdf", FileMode.CreateNew, FileAccess.Write);
                doc.Docsignedpath = doc.Docsignedpath + "-signed.pdf";
                PdfStamper stp = PdfStamper.CreateSignature(reader, fos, '\0', null, true); 
                Org.BouncyCastle.X509.X509Certificate[] chain = crearCertificado();
                AsymmetricKeyParameter pk = readPrivateKey();
                stp.Writer.CloseStream = false;
                LtvVerification v = stp.LtvVerification;
                AcroFields af = stp.AcroFields;
                foreach (String sigName in af.GetSignatureNames()) 
                {                   
                    v.AddVerification(sigName, new OcspClientBouncyCastle(), new CrlClientOffline(null), LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO);
                }
                PdfSignatureAppearance sap = stp.SignatureAppearance;
                sap.Reason = "";
                sap.Location = "";
                //Preserve some space for the contents
                int contentEstimated = 15000;
                Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
                exc.Add(PdfName.CONTENTS, (contentEstimated * 2 + 2));
                //Add timestamp
                TSAClientBouncyCastle tsc  = new TSAClientBouncyCastle(Properties.Settings.Default.tsaUrl, Properties.Settings.Default.tsaUser, Properties.Settings.Default.tsaPass, contentEstimated, DigestAlgorithms.SHA512);
                // Creating the signature
                LtvTimestamp.Timestamp(sap, tsc, null);
                //Org.BouncyCastle.Crypto.BouncyCastleDigest messageDigest = MessageDigest.getInstance("SHA1");
                //IExternalDigest digest = new Org.BouncyCastle.Crypto.BouncyCastleDigest();
                //RSACryptoServiceProvider crypt = (RSACryptoServiceProvider)cert.PrivateKey;
                IExternalSignature signature = new PrivateKeySignature(pk, DigestAlgorithms.SHA512);
                MakeSignature.SignDetached(sap, signature, chain, null, null, tsc, 0, CryptoStandard.CMS);
                stp.Close();
                fos.Close();
                reader.Close();
            }
            catch (IOException ex)
            {
                Log.Error("IOException", ex);
            }
            catch (DocumentException dex)
            {
                Log.Error("DocumentException", dex);
            }
        }
        /**
         * @Method
         * @Params
         * @Return
         */
        public static String readResourceFile(String fileName)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;
            _assembly = Assembly.GetExecutingAssembly();
            _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream(fileName));
            return _textStreamReader.ReadToEnd();
        }
    }
}
