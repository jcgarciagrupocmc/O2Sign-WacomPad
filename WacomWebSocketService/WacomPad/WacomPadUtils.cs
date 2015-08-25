using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WacomWebSocketService.Entities;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool BringToFrontCustom(Form f)
        {
            bool toReturn = true;
            try
            {
                //This is the same as the name of the executable without the .exe at the end            
                Process[] processes = Process.GetProcessesByName(f.Name);

                SetForegroundWindow(processes[0].MainWindowHandle);
                f.BringToFront();
            }
            catch (Exception e)
            {
                toReturn = false;
                //MessageBox.Show("Something went wrong, Please bring the window to front manually");
            }
            return toReturn;
        }
    }
}
