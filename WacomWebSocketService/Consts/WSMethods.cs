using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Consts
{
    class WSMethods
    {
        public static readonly String GET_PDF_BY_ID = "/getpdfbyid/{0}";
        public static readonly String GET_DOCS_BY_OPERATION = "/getdocsbyoperation/{0}";
        public static readonly String UPLOAD_SIGNED_PDF = "/uploadsignedpdf";
    }
}
