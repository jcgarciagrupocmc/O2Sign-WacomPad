using System;

namespace WacomWebSocketService.Exceptions
{
    class IncorrectMessageException : System.Exception
    {
        public string response;

        public System.Exception ex;

        public IncorrectMessageException(System.Exception ex, String jsonResponse)
        {
            this.response = jsonResponse;
            this.ex = ex;
            this.Source = ex.Source;
            this.HResult = ex.HResult;
        }
    }
}
