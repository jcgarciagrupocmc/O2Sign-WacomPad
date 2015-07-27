using System;


namespace WacomWebSocketService.JSONMessageConverter
{
    abstract class MessageConverter
    {
        internal abstract string recieveMessage(String message);
    }
}
