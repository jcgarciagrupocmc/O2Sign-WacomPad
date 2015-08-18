using System;

namespace WacomWebSocketService.Entities
{
    public class BioSignPoint
    {
        private int order;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        private ushort x;

        public ushort X
        {
            get { return x; }
            set { x = value; }
        }
        private ushort y;

        public ushort Y
        {
            get { return y; }
            set { y = value; }
        }
        private byte sw;

        public byte Sw
        {
            get { return sw; }
            set { sw = value; }
        }
        private bool rdy;

        public bool Rdy
        {
            get { return rdy; }
            set { rdy = value; }
        }
        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        private ushort pressure;

        public ushort Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }

    }
}
