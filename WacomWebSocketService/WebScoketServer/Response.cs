using WacomWebSocketService.Consts;

namespace WacomWebSocketService.WebScoketServer
{
    public class Response
    {
        // Response Type
        public ResponseType Type { get; set; }
        // Response Data
        public dynamic Data { get; set; }

        public string Data2 { get; set; }
    }
}
