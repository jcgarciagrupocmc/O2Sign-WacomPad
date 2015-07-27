using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Consts
{

    /**
     * Enum that specifies the Response Type of a WebSocket Response
     */
    public enum ResponseType
    {
        Error = -1,
        Connection = 0,
        Disconnect = 1,
        Message = 2,
        NewFile = 3,
        FileRecieved =4,
        DataRecieved =5
    }
}
