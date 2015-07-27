using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Consts;

namespace WacomWebSocketService.WebScoketServer
{
    public class Response
    {
        public ResponseType Type { get; set; }
        public dynamic Data { get; set; }
    }
}
