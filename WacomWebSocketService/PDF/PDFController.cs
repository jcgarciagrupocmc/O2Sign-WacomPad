using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using WacomWebSocketService.Entities;
using iTextSharp.text.pdf.security;

namespace WacomWebSocketService.PDF
{
    class PDFController
    {
        private PdfWriter writer;
        private FileStream fos;
        private Document doc;

        /**
         * @Method open a existing pdf file
         * @Param path path of pdfFile
         * @Return returns true if the file was opened or false if file couldn't be opened
         */
        private bool open(String path)
        {
            if (File.Exists(path))
            {
                doc = new Document();
                fos = new FileStream(path, FileMode.Open);
                writer = PdfWriter.GetInstance(doc, fos);
                return true;
            }
            return false;
        }

        /**
         * Close a previous opnened pdf document
         */
        private void close()
        {
            writer.Close();
            doc.Close();
            fos.Close();
        }

        /**
         * 
         * 
         * 
         * 
         * 
         */
        public DocumentData doSignature(DocumentData source)
        {
            this.open(source.Docpath);
            //TODO some logic
            
            this.close();
            throw new NotImplementedException();

        }

        private DocumentData doPAdES(GraphSign signature, DocumentData coordinates)
        {
            DocumentData result = new DocumentData();
            result.Docname = coordinates.Docname;

            TSAClientBouncyCastle TSAClient = new TSAClientBouncyCastle(Properties.Settings.Default.tsaUrl, Properties.Settings.Default.tsaUser, Properties.Settings.Default.tsaPass);
            PdfReader reader = new PdfReader(coordinates.Docpath);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, this.fos, '\0', null, true);
            PdfSignatureAppearance sap = stamper.SignatureAppearance;             //sap.SignDate =

            throw new NotImplementedException();

        }
    }
}
