using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.WacomPad
{
    class WacomPadUtils
    {
        public static BioSignPoint toBioSignPoint(wgssSTU.IPenData penData)
        {
            BioSignPoint result = new BioSignPoint();
            result.Pressure = penData.pressure;
            result.Rdy = penData.rdy;
            result.Sw = penData.sw;
            result.X = penData.x;
            result.Y = penData.y;
            result.Time = System.DateTime.Now;
            return result;
        }

        public static wgssSTU.IPenData toIPenData(BioSignPoint point)
        {
            //wgssSTU.IPenData result = new wgssSTU.
            //result.pressure = point.Pressure;
            throw new NotImplementedException();


        }
    }
}
