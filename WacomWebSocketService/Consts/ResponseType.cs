
namespace WacomWebSocketService.Consts
{

    /**
     * Enum that specifies the Response Type of a WebSocket Response
     */
    public enum ResponseType
    {

        //ResponseMessage for any error, error as paramenter
        Error = -1,
        //ResponseMessage for a Register Command OK
        Connection = 0,
        //ResponseMessage for a Disconnected Command OK
        Disconnect = 1,
        //ResponseMessage for a GetOperation Command, List<DocumentData> json object as parameter
        OperationList = 2,
        //ResponseMessage for GetFile and GetSignedFile Commands, PDF file in Base64 as parameter
        File = 3,
        //ResponseMessage for a SignFile Command OK
        SignedFile = 4,
        //ResponseMessage for a UploadOperation Command OK
        OperationOK = 5,
        //ResponseMessage for a CancelOperation Command OK
        OperationCanceled = 6
    }
}
