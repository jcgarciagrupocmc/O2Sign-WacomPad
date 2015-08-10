using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Consts
{
    /**
     * Enum to type the messages recieved from WebSocket
     */
    public enum CommandType
    {
        //Message Type for connecting
        Register = 0,
        //Message Type for disconnecting
        Disconnect = 1,
        //Message Type for getting an operation from PIE, parameter required idOperation
        GetOperation = 2,
        //Message Type for getting an operation unsigned PDF file for visualizing, parameter required document uuid
        GetFile = 3,
        //Message Type for starting signing process of selected PDF Document, parameter required document uuid
        SignFile = 4,
        //Message Type for getting an operation signed PDF file for visualizing, parameter required document uuid
        GetSignedFile = 5,
        //Message Type for sending a completed operation to the PIE, parameter required idoperation
        UploadOperation = 6,
        //Message Type for cancelling current operation, parameter required idoperation
        CancelOperation = 7
    }
}
