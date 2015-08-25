using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using WacomWebSocketService.Entities;
using log4net;
using System.Collections.Generic;

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
            String path;
            pdfDoc.Docsignedpath = Properties.Settings.Default.tempPath + pdfDoc.Idoperation + "\\signed\\" + pdfDoc.Docname;
            if (pdfDoc.documentHasBeenSigned())
            {
                path = pdfDoc.Docpath + "-signed.pdf";
                //signedpath = 
            }
            else
            {
                path = pdfDoc.Docpath;
            }
            if (File.Exists(pdfDoc.Docpath))
            {
                doc = new Document();
                reader = new PdfReader(path);
                fos = new FileStream(pdfDoc.Docsignedpath, FileMode.Create, FileAccess.Write);
                writer = PdfWriter.GetInstance(doc, fos);
                doc.Open();
                doc.AddDocListener(writer);
                return true;
            }
            return false;
        }

        /**
         * @Method Close a previous opnened pdf document
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
         * @Params signer data
         * @Return true if the document is correctly signed, false if something wrong
         */
        public bool doSignature(DocumentData source,GraphSign sign, string metadata, Signer signer)
        {
            bool result = false;
            bool insertedSign = false;
            if (this.open(source))
            {
                Dictionary<String, String> hMap = this.reader.Info;
                String keywords = "";
                hMap.TryGetValue("Keywords",out keywords);
                keywords += metadata+Properties.Settings.Default.stringSeparator;
                hMap.Remove("Keywords");
                hMap.Add("Keywords", keywords);
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
                    if (i == signer.Page)
                        insertedSign = this.insertGraphSign(sign, cb, signer.X, signer.Y);
                }
                if (insertedSign)
                {
                    //Do PAdES
                    this.close();
                    DigitalSignUtils.signPDF(source, hMap);
                    if(File.Exists(source.Docpath+"-signed.pdf"))
                        File.Delete(source.Docpath+"-signed.pdf");
                    File.Copy(source.Docsignedpath, source.Docpath + "-signed.pdf");
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
                sign.Image.MakeTransparent(System.Drawing.Color.White);
                sign.Image.Save(Properties.Settings.Default.tempPath + "test.png", System.Drawing.Imaging.ImageFormat.Png);                
                Image i = Image.GetInstance((System.Drawing.Image)sign.Image, new BaseColor(System.Drawing.Color.Transparent));
                //i.BackgroundColor = BaseColor.
                float percentage = float.Parse(Properties.Settings.Default.imageRatio)/100;
                //float percentage = 0.33f;
                page.AddImage(i, i.Width*percentage, 0, 0, i.Height*percentage, x, y);
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
        
        /**
         * @Method Method that inserts the GraphSign in the pdf document and call for signing it
         * @Params DocumentData source document to be signed
         * @Params GraphSign sign Image and metadata about the Graphical Sign
         * @Params signArray String Array with Graphometric info
         * @Params signer data
         * @Return true if the document is correctly signed, false if something wrong
         */
        internal bool doSignature(DocumentData doc, GraphSign sign, string[] signArray, Signer signer)
        {
            String encrypted ="";
            foreach (String s in signArray)
                encrypted += s;
            encrypted = DigitalSignUtils.encrypt(encrypted);
            
            return this.doSignature(doc, sign, encrypted,signer);
        }
        /**
        * @Method Method that inserts the GraphSign in the pdf document and call for signing it
        * @Params DocumentData source document to be signed
        * @Params GraphSign sign Image and metadata about the Graphical Sign
        * @Params jsonSign Graphometric info JSON serialized
        * @Params signer data
        * @Return true if the document is correctly signed, false if something wrong
        */
        internal bool doSignature(DocumentData doc, GraphSign sign, String jsonSign, Signer signer, bool b)
        {
            String encrypted = jsonSign;
            return this.doSignature(doc, sign, encrypted,signer);
        }
    }
}
