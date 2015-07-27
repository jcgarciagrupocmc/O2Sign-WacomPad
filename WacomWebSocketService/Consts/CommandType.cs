using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WacomWebSocketService.Consts
{
    public enum CommandType
    {
        Register = 0,
        Disconnect = 1,
        GetOperation = 2,
        GetFile = 3,
        FileRecieved = 4
    }
}
