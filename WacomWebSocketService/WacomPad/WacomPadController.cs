using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using WacomWebSocketService.Entities;
using System.Windows.Forms;


namespace WacomWebSocketService.WacomPad
{
    public class WacomPadController
    {

        public GraphSign padSigning()
        {
            try
            {
                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                WacomPadForm demo = new WacomPadForm(usbDevice);

                demo.ShowDialog();
                return demo.getSign();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool checkPadConnected()
        {
            wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
            return (usbDevices.Count != 0);
        }
    }   
}
