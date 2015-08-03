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
using log4net;

namespace WacomWebSocketService.PDF
{
    public class PDFController
    {
        private PdfWriter writer;
        private PdfReader reader;
        private FileStream fos;
        private Document doc;
        private ILog Log;


        public PDFController()
        {
            this.Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        /**
         * @Method open a existing pdf file
         * @Param path path of pdfFile
         * @Return returns true if the file was opened or false if file couldn't be opened
         */
        private bool open(DocumentData pdfDoc)
        {
            if (File.Exists(pdfDoc.Docpath))
            {
                doc = new Document();
                reader = new PdfReader(pdfDoc.Docpath);
                fos = new FileStream(pdfDoc.Docsignedpath, FileMode.Create,FileAccess.Write);
                writer = PdfWriter.GetInstance(doc, fos);
                doc.Open();
                doc.AddDocListener(writer);
                return true;
            }
            return false;
        }

        /**
         * Close a previous opnened pdf document
         */
        private void close()
        {
            doc.Close();
            fos.Close();
            writer.Close();
            reader.Close();
        }

        /**
         * 
         * 
         * 
         * 
         * 
         */
        public bool doSignature(DocumentData source,GraphSign sign)
        {
            bool result = false;
            bool insertedSign = false;
            if (this.open(source))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    doc.SetPageSize(reader.GetPageSize(i));
                    doc.NewPage();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage importedPage = writer.GetImportedPage(reader, i);
                    
                    int rotation = reader.GetPageRotation(i);
                    if (rotation == 90 || rotation == 270)
                        cb.AddTemplate(importedPage, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                    else
                        cb.AddTemplate(importedPage, 1.0F, 0, 0, 1.0F, 0, 0);
                    if (i == source.Page)
                        insertedSign = this.insertGraphSign(sign, cb, source.X, source.Y);
                }
                if (insertedSign)
                {
                    this.close();
                    DigitalSignUtils.signPDF(source);
                    result = true;

                }
            }
            
            this.close();
            return result;
            throw new NotImplementedException();

        }
        /**
         * 
         * 
         */
        private bool insertGraphSign(GraphSign sign,PdfContentByte page, int x, int y)
        {
            try
            {
                Image i = Image.GetInstance((System.Drawing.Image)sign.Image, BaseColor.WHITE);
                page.AddImage(i, i.Width, 0, 0, i.Height, x, y);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return false;
            }
        }
        /**
         * 
         * 
         */
        private DocumentData doPAdES(GraphSign signature, DocumentData coordinates)
        {
            DocumentData result = new DocumentData();
            result.Docname = coordinates.Docname;

            TSAClientBouncyCastle TSAClient = new TSAClientBouncyCastle(Properties.Settings.Default.tsaUrl, Properties.Settings.Default.tsaUser, Properties.Settings.Default.tsaPass);
            PdfReader reader = new PdfReader(coordinates.Docpath);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, this.fos, '\0', null, true);
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.SetVisibleSignature("");

            //sap.SignDate =

            throw new NotImplementedException();

        }
    }
}
