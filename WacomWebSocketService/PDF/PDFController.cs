using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using WacomWebSocketService.Entities;
using log4net;

namespace WacomWebSocketService.PDF
{

    /**
     *@Class Class reponsable of all PDF read/write and signing operations
     */
    public class PDFController
    {
        private PdfWriter writer;
        private PdfReader reader;
        private FileStream fos;
        private Document doc;
        private ILog Log;

        /**
         * @Method Default constructor initializes the logger.
         */
        public PDFController()
        {
            if (LogManager.GetCurrentLoggers().Length > 0)
                this.Log = LogManager.GetCurrentLoggers()[0];
            else
                this.Log = LogManager.GetLogger(Properties.Settings.Default.logName);
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
         * @Method Method that inserts the GraphSign in the pdf document and call for signing it
         * @Params DocumentData source document to be signed
         * @Params GraphSign sign Image and metadata about the Graphical Sign
         * @Return true if the document is correctly signed, false if something wrong
         */
        public bool doSignature(DocumentData source,GraphSign sign)
        {
            bool result = false;
            bool insertedSign = false;
            if (this.open(source))
            {
                //Copy PDF
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
                    //Insert Graph image on coordenates
                    if (i == source.Page)
                        insertedSign = this.insertGraphSign(sign, cb, source.X, source.Y);
                }
                if (insertedSign)
                {
                    //Do PAdES
                    this.close();
                    DigitalSignUtils.signPDF(source);
                    result = true;
                }
            }            
            this.close();
            return result;
        }
        /**
         * @Method insert Sign Image in coordenates
         * @Params GraphSign sign biometrical graph sign
         * @Params PdfContentByte page where image will be inserted
         * @Params int x X Coordinate
         * @Params int y Y Coordinate
         * @Return true if all its OK, false if something wrong
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
                //Logging exception
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return false;
            }
        }
        
        ///**
        // * 
        // * 
        // */
        //private DocumentData doPAdES(GraphSign signature, DocumentData coordinates)
        //{
        //    DocumentData result = new DocumentData();
        //    result.Docname = coordinates.Docname;

        //    TSAClientBouncyCastle TSAClient = new TSAClientBouncyCastle(Properties.Settings.Default.tsaUrl, Properties.Settings.Default.tsaUser, Properties.Settings.Default.tsaPass);
        //    PdfReader reader = new PdfReader(coordinates.Docpath);
        //    PdfStamper stamper = PdfStamper.CreateSignature(reader, this.fos, '\0', null, true);
        //    PdfSignatureAppearance appearance = stamper.SignatureAppearance;
        //    appearance.SetVisibleSignature("");

        //    //sap.SignDate =

        //    throw new NotImplementedException();

        //}
    }
}
