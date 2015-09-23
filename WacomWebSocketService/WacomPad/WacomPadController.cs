using System;
using WacomWebSocketService.Entities;
using log4net;


namespace WacomWebSocketService.WacomPad
{
    /**
     * @Class class that controlls Wacom Pad Logic
     */
    public class WacomPadController
    {
        private int minPoints;

        public int MinPoints
        {
            get { return minPoints; }
            set { minPoints = value; }
        }
        public WacomPadController()
        {
            this.minPoints = 100;
        }
        /**
         * @Method calls the Wacom Pad UI for signing and returns the sign data
         * @Return GraphSign if confirmed, null if cancel by the user
         */
        public GraphSign padSigning(Signer signer)
        {

            ILog Log;
            Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            try
            {

                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                WacomPadForm.PresingString = String.Format(Properties.Settings.Default.presingModel, signer.Nombre, signer.Nif);
                WacomPadForm demo = new WacomPadForm(usbDevice,this.minPoints);
                demo.Title = String.Format(Properties.Settings.Default.presingModel, signer.Nombre, signer.Nif);
                //WacomPadUtils.BringToFrontCustom(demo);
                demo.ShowDialog();
                GraphSign result = demo.getSign();
                demo.Dispose();
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return null;
            }
        }
        /**
         * @Method Checks if a Wacom Pad is connected and ready in the system
         * @Return true if Wacom Pad is available, false otherwise
         */
        public bool checkPadConnected()
        {
            ILog Log;
            Log = LogManager.GetLogger(Properties.Settings.Default.logName);

            try
            {
                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                return (usbDevices.Count != 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
    }   
}
