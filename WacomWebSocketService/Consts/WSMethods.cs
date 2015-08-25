using System;

namespace WacomWebSocketService.Consts
{
    /**
     * @Class Constants of REST Services WebMethods
     */
    class WSMethods
    {
        // /getpdfbyid REST service parameterizable URL
        public static readonly String GET_PDF_BY_ID = "/getpdfbyid/{0}";
        // /getdocsbyoperation REST service parameterizable URL
        public static readonly String GET_DOCS_BY_OPERATION = "/getdocsbyoperation/{0}";
        // /uploadsignedpdf REST service URL
        public static readonly String UPLOAD_SIGNED_PDF = "/uploadsignedpdf";
        // /uploadallsignedpdf REST service URL
        public static readonly String UPLOAD_ALL_SIGNED_PDF = "/uploadallsignedpdf";
    }
}
