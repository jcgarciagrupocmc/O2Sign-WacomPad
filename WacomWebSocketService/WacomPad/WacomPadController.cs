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
        /**
         * @Method calls the Wacom Pad UI for signing and returns the sign data
         * @Return GraphSign if confirmed, null if cancel by the user
         */
        public GraphSign padSigning(Signer signer)
        {

            ILog Log;
            //if (LogManager.GetCurrentLoggers().Length > 0)
            //    Log = LogManager.GetCurrentLoggers()[0];
            //else
                Log = LogManager.GetLogger(Properties.Settings.Default.logName);
            try
            {

                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                WacomPadForm.PresingString = String.Format(Properties.Settings.Default.presingModel, signer.Nombre, signer.Nif);
                WacomPadForm demo = new WacomPadForm(usbDevice);
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
                throw ex;
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
            //if (LogManager.GetCurrentLoggers().Length > 0)
            //    Log = LogManager.GetCurrentLoggers()[0];
            //else
                Log = LogManager.GetLogger(Properties.Settings.Default.logName);

            try
            {
                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                return (usbDevices.Count != 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new Exception("error COM", ex);
                return false;
            }
        }
    }   
}
