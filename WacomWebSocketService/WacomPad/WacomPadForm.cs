﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.WacomPad
{
    public partial class WacomPadForm : Form
    {
        private wgssSTU.Tablet m_tablet;
        private wgssSTU.ICapability m_capability;
        private wgssSTU.IInformation m_information;

        private Button presignText;
        private int minPoints;

        private bool semaphore;

        private static String presingString;

        public static String PresingString
        {
            get { return presingString; }
            set { presingString = value; }
        }
       


        private String title;

        public String Title
        {
            get { return title; }
            set { title = value; }
        }
        // In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
        // Using an array of these makes it easy to add or remove buttons as desired.
        private delegate void ButtonClick();
        private struct Button
        {
            public Rectangle Bounds; // in Screen coordinates
            public String Text;
            public EventHandler Click;

            public void PerformClick()
            {
                Click(this, null);
            }
        };


        private Pen m_penInk;  // cached object.

        // The isDown flag is used like this:
        // 0 = up
        // +ve = down, pressed on button number
        // -1 = down, inking
        // -2 = down, ignoring
        private int m_isDown;

        private GraphSign sign;

        private Button[] m_btns; // The array of buttons that we are emulating.

        private Bitmap m_bitmap; // This bitmap that we display on the screen.
        private wgssSTU.encodingMode m_encodingMode; // How we send the bitmap to the device.
        private byte[] m_bitmapData; // This is the flattened data of the bitmap that we send to the device.

        // As per the file comment, there are three coordinate systems to deal with.
        // To help understand, we have left the calculations in place rather than optimise them.

        //Disable close button
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private PointF tabletToClient(BioSignPoint penData)
        {
            // Client means the Windows Form coordinates.
            return new PointF((float)penData.X * this.ClientSize.Width / m_capability.tabletMaxX, (float)penData.Y * this.ClientSize.Height / m_capability.tabletMaxY);
        }

        private int order;

        private Point tabletToScreen(BioSignPoint penData)
        {
            // Screen means LCD screen of the tablet.
            return Point.Round(new PointF((float)penData.X * m_capability.screenWidth / m_capability.tabletMaxX, (float)penData.Y * m_capability.screenHeight / m_capability.tabletMaxY));
        }
        private Point clientToScreen(Point pt)
        {
            // client (window) coordinates to LCD screen coordinates. 
            // This is needed for converting mouse coordinates into LCD bitmap coordinates as that's
            // what this application uses as the coordinate space for buttons.
            return Point.Round(new PointF((float)pt.X * m_capability.screenWidth / this.ClientSize.Width, (float)pt.Y * m_capability.screenHeight / this.ClientSize.Height));
        }


        private void clearScreen()
        {
            // note: There is no need to clear the tablet screen prior to writing an image.
            m_tablet.writeImage((byte)m_encodingMode, m_bitmapData);
            m_isDown = 0;
            this.Invalidate();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            if (semaphore)
            {
                semaphore = false;
                if (this.sign != null)
                {
                    if (this.sign.Points.Count > minPoints)
                    {
                        // Save the image.
                        SaveImage();

                        m_tablet.writeImage((byte)m_encodingMode, m_bitmapData);
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Close();
                        });
                    }
                    else
                    {
                        MessageBox.Show("Firma demasiado corta", "Alerta");
                        this.sign.ClearPoints();
                        this.clearScreen();

                    }
                }
                else
                {
                    MessageBox.Show("Firma demasiado corta", "Alerta");
                }
                semaphore = true;

            }
            //this.Hide();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            // You probably want to add additional processing here.
            this.sign = null;

            m_tablet.writeImage((byte)m_encodingMode, m_bitmapData);
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            if (this.sign != null)
            {

                this.sign.ClearPoints();                
            }           
                clearScreen();
        }

        public WacomPadForm(wgssSTU.IUsbDevice usbDevice, int points)
        {

            semaphore = true;
            this.minPoints = points;
            if (presingString != null)
            {
                presignText.Text = presingString;
            }
            this.sign = new GraphSign();
            // This is a DPI aware application, so ensure you understand how .NET client coordinates work.
            // Testing using a Windows installation set to a high DPI is recommended to understand how
            // values get scaled or not.
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            InitializeComponent();

            m_tablet = new wgssSTU.Tablet();
            wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();

            // A more sophisticated applications should cycle for a few times as the connection may only be
            // temporarily unavailable for a second or so. 
            // For example, if a background process such as Wacom STU Display
            // is running, this periodically updates a slideshow of images to the device.

            wgssSTU.IErrorCode ec = m_tablet.usbConnect(usbDevice, true);
            if (ec.value == 0)
            {
                m_capability = m_tablet.getCapability();
                m_information = m_tablet.getInformation();
            }
            else
            {
                throw new Exception(ec.message);
            }

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            // Set the size of the client window to be actual size, 
            // based on the reported DPI of the monitor.

            Size clientSize = new Size((int)(m_capability.tabletMaxX / 2540F * 96F), (int)(m_capability.tabletMaxY / 2540F * 96F));
            this.ClientSize = clientSize;
            this.ResumeLayout();

            m_btns = new Button[3];
            if (usbDevice.idProduct != 0x00a2)
            {
                // Place the buttons across the bottom of the screen.

                int w2 = m_capability.screenWidth / 3;
                int w3 = m_capability.screenWidth / 3;
                int w1 = m_capability.screenWidth - w2 - w3;
                int y = m_capability.screenHeight * 6 / 7;
                int h = m_capability.screenHeight - y;

                presignText.Bounds = new Rectangle(0, 0, m_capability.screenWidth, h);
                m_btns[0].Bounds = new Rectangle(0, y, w1, h);
                m_btns[1].Bounds = new Rectangle(w1, y, w2, h);
                m_btns[2].Bounds = new Rectangle(w1 + w2, y, w3, h);
            }
            else
            {
                // The STU-300 is very shallow, so it is better to utilise
                // the buttons to the side of the display instead.

                int x = m_capability.screenWidth * 3 / 4;
                int w = m_capability.screenWidth - x;

                int h2 = m_capability.screenHeight / 3;
                int h3 = m_capability.screenHeight / 3;
                int h1 = m_capability.screenHeight - h2 - h3;

                m_btns[0].Bounds = new Rectangle(x, 0, w, h1);
                m_btns[1].Bounds = new Rectangle(x, h1, w, h2);
                m_btns[2].Bounds = new Rectangle(x, h1 + h2, w, h3);
            }
            m_btns[0].Text = "Aceptar";
            m_btns[1].Text = "Borrar";
            m_btns[2].Text = "Cancelar";
            m_btns[0].Click = new EventHandler(btnOk_Click);
            m_btns[1].Click = new EventHandler(btnClear_Click);
            m_btns[2].Click = new EventHandler(btnCancel_Click);


            // Disable color if the STU-520 bulk driver isn't installed.
            // This isn't necessary, but uploading colour images with out the driver
            // is very slow.

            // Calculate the encodingMode that will be used to update the image
            ushort idP = m_tablet.getProductId();
            wgssSTU.encodingFlag encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(idP, 0);
            bool useColor = false;
            if ((encodingFlag & (wgssSTU.encodingFlag.EncodingFlag_16bit | wgssSTU.encodingFlag.EncodingFlag_24bit)) != 0)
            {
                if (m_tablet.supportsWrite())
                    useColor = true;
            }
            if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_24bit) != 0)
            {
                m_encodingMode = m_tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_24bit_Bulk : wgssSTU.encodingMode.EncodingMode_24bit;
            }
            else if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_16bit) != 0)
            {
                m_encodingMode = m_tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_16bit_Bulk : wgssSTU.encodingMode.EncodingMode_16bit;
            }
            else
            {
                // assumes 1bit is available
                m_encodingMode = wgssSTU.encodingMode.EncodingMode_1bit;
            }

            // Size the bitmap to the size of the LCD screen.
            // This application uses the same bitmap for both the screen and client (window).
            // However, at high DPI, this bitmap will be stretch and it would be better to 
            // create individual bitmaps for screen and client at native resolutions.
            m_bitmap = new Bitmap(m_capability.screenWidth, m_capability.screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            {
                Graphics gfx = Graphics.FromImage(m_bitmap);
                gfx.Clear(Color.White);

                // Uses pixels for units as DPI won't be accurate for tablet LCD.
                Font font = new Font(FontFamily.GenericSansSerif, m_btns[0].Bounds.Height / 2F, GraphicsUnit.Pixel);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                if (useColor)
                {
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    // Anti-aliasing should be turned off for monochrome devices.
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                }

                gfx.DrawRectangle(Pens.Black, presignText.Bounds);
                gfx.DrawString(presignText.Text, font, Brushes.Black, presignText.Bounds, sf);

                // Draw the buttons
                for (int i = 0; i < m_btns.Length; ++i)
                {
                    if (useColor)
                    {
                        gfx.FillRectangle(Brushes.LightGray, m_btns[i].Bounds);
                    }
                    gfx.DrawRectangle(Pens.Black, m_btns[i].Bounds);
                    gfx.DrawString(m_btns[i].Text, font, Brushes.Black, m_btns[i].Bounds, sf);
                }

                gfx.Dispose();
                font.Dispose();

                // Finally, use this bitmap for the window background.
                this.BackgroundImage = m_bitmap;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Now the bitmap has been created, it needs to be converted to device-native
            // format.
            {

                // Unfortunately it is not possible for the native COM component to
                // understand .NET bitmaps. We have therefore convert the .NET bitmap
                // into a memory blob that will be understood by COM.

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                m_bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                m_bitmapData = (byte[])protocolHelper.resizeAndFlatten(stream.ToArray(), 0, 0, (uint)m_bitmap.Width, (uint)m_bitmap.Height, m_capability.screenWidth, m_capability.screenHeight, (byte)m_encodingMode, wgssSTU.Scale.Scale_Fit, 0, 0);
                protocolHelper = null;
                stream.Dispose();
            }

            // If you wish to further optimize image transfer, you can compress the image using 
            // the Zlib algorithm.

            bool useZlibCompression = false;
            if (!useColor && useZlibCompression)
            {
                // m_bitmapData = compress_using_zlib(m_bitmapData); // insert compression here!
                m_encodingMode |= wgssSTU.encodingMode.EncodingMode_Zlib;
            }

            // Calculate the size and cache the inking pen.

            SizeF s = this.AutoScaleDimensions;
            float inkWidthMM = 0.7F;
            m_penInk = new Pen(Color.DarkBlue, inkWidthMM / 25.4F * ((s.Width + s.Height) / 2F));
            m_penInk.StartCap = m_penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            m_penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;


            // Add the delagate that receives pen data.
            m_tablet.onPenData += new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
            m_tablet.onGetReportException += new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);


            // Initialize the screen
            clearScreen();

            // Enable the pen data on the screen (if not already)
            m_tablet.setInkingMode(0x01);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Ensure that you correctly disconnect from the tablet, otherwise you are 
            // likely to get errors when wanting to connect a second time.
            if (m_tablet != null)
            {
                m_tablet.onPenData -= new wgssSTU.ITabletEvents2_onPenDataEventHandler(onPenData);
                m_tablet.onGetReportException -= new wgssSTU.ITabletEvents2_onGetReportExceptionEventHandler(onGetReportException);
                m_tablet.setInkingMode(0x00);
                m_tablet.setClearScreen();
                m_tablet.disconnect();
            }

            m_penInk.Dispose();
        }

        private void onGetReportException(wgssSTU.ITabletEventsException tabletEventsException)
        {
            try
            {
                tabletEventsException.getException();
            }
            catch (Exception e)
            {
                //TODO Log4NET
                m_tablet.disconnect();
                m_tablet = null;
                this.sign = null;
                this.Close();
            }
        }

        private void onPenData(wgssSTU.IPenData penData) // Process incoming pen data
        {
            Point pt = tabletToScreen(WacomPadUtils.toBioSignPoint(penData));

            int btn = 0; // will be +ve if the pen is over a button.
            {
                for (int i = 0; i < m_btns.Length; ++i)
                {
                    if (m_btns[i].Bounds.Contains(pt))
                    {
                        btn = i + 1;
                        break;
                    }
                }
            }

            bool isDown = (penData.sw != 0);

            // This code uses a model of four states the pen can be in:
            // down or up, and whether this is the first sample of that state.

            if (isDown)
            {
                if (m_isDown == 0)
                {
                    // transition to down
                    if (btn > 0)
                    {
                        // We have put the pen down on a button.
                        // Track the pen without inking on the client.

                        m_isDown = btn;
                    }
                    else
                    {
                        // We have put the pen down somewhere else.
                        // Treat it as part of the signature.

                        m_isDown = -1;
                    }
                }
                else
                {
                    // already down, keep doing what we're doing!
                }

                // draw
                if (this.sign.Points.Count != 0 && m_isDown == -1)
                {
                    // Draw a line from the previous down point to this down point.
                    // This is the simplist thing you can do; a more sophisticated program
                    // can perform higher quality rendering than this!

                    Graphics gfx = this.CreateGraphics();
                    gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    BioSignPoint prevPenData = this.sign.Points[this.sign.Points.Count - 1];

                    PointF prev = tabletToClient(prevPenData);

                    gfx.DrawLine(m_penInk, prev, tabletToClient(WacomPadUtils.toBioSignPoint(penData)));
                    gfx.Dispose();
                }

                // The pen is down, store it for use later.
                if (m_isDown == -1)
                    this.sign.addPoint(WacomPadUtils.toBioSignPoint(penData));
            }
            else
            {
                if (m_isDown != 0)
                {
                    // transition to up
                    if (btn > 0)
                    {
                        // The pen is over a button

                        if (btn == m_isDown)
                        {
                            // The pen was pressed down over the same button as is was lifted now. 
                            // Consider that as a click!
                            m_btns[btn - 1].PerformClick();
                        }
                    }
                    m_isDown = 0;
                }
                else
                {
                    // still up
                }

                // Add up data once we have collected some down data.
                if ((this.sign!= null)&&(this.sign.Points.Count != 0))
                    this.sign.addPoint(WacomPadUtils.toBioSignPoint(penData));
            }
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (this.sign.Points.Count != 0)
            {
                // Redraw all the pen data up until now!

                Graphics gfx = e.Graphics;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                bool isDown = false;
                PointF prev = new PointF();
                for (int i = 0; i < this.sign.Points.Count; ++i)
                {
                    if (this.sign.Points[i].Sw != 0)
                    {
                        if (!isDown)
                        {
                            isDown = true;
                            prev = tabletToClient(this.sign.Points[i]);
                        }
                        else
                        {
                            PointF curr = tabletToClient(this.sign.Points[i]);
                            gfx.DrawLine(m_penInk, prev, curr);
                            prev = curr;
                        }
                    }
                    else
                    {
                        if (isDown)
                        {
                            isDown = false;
                        }
                    }
                }
            }

        }

        private void Form2_MouseClick(object sender, MouseEventArgs e)
        {
            // Enable the mouse to click on the simulated buttons that we have displayed.

            // Note that this can add some tricky logic into processing pen data
            // if the pen was down at the time of this click, especially if the pen was logically
            // also 'pressing' a button! This demo however ignores any that.

            Point pt = clientToScreen(e.Location);
            foreach (Button btn in m_btns)
            {
                if (btn.Bounds.Contains(pt))
                {
                    btn.PerformClick();
                    break;
                }
            }
        }


        public wgssSTU.ICapability getCapability()
        {
            return this.sign != null ? m_capability : null;
        }

        public wgssSTU.IInformation getInformation()
        {
            return this.sign != null ? m_information : null;
        }

        // Save the image in a local file
        private void SaveImage()
        {
            try
            {
                Bitmap bitmap = GetImage(new Rectangle(0, 0, m_capability.screenWidth, m_capability.screenHeight));
                this.sign.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message+" "+ex.Source);
            }
        }

        // Draw an image with the existed points.
        public Bitmap GetImage(Rectangle rect)
        {
            Bitmap retVal = null;
            Bitmap bitmap = null;
            SolidBrush brush = null;
            try
            {
                bitmap = new Bitmap(rect.Width, rect.Height);
                Graphics graphics = Graphics.FromImage(bitmap);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                brush = new SolidBrush(Color.White);
                graphics.FillRectangle(brush, 0, 0, rect.Width, rect.Height);

                for (int i = 1; i < this.sign.Points.Count; i++)
                {
                    PointF p1 = tabletToScreen(this.sign.Points[i - 1]);
                    PointF p2 = tabletToScreen(this.sign.Points[i]);

                    if (this.sign.Points[i - 1].Sw > 0 || this.sign.Points[i].Sw > 0)
                    {
                        graphics.DrawLine(m_penInk, p1, p2);
                    }
                }

                retVal = bitmap;
                bitmap = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception2: " + ex.Message + " " + ex.Source);
            }
            finally
            {
                if (brush != null)
                    brush.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
            }
            return retVal;
        }

        public GraphSign getSign()
        {
            return this.sign;
        }

        private void WacomPadForm_Shown(object sender, EventArgs e)
        {
            this.Text = this.title;
            //WacomPadUtils.BringToFrontCustom(this);
            this.BringToFront();
        }

        //private void Form2_MouseClick(object sender, MouseEventArgs e)
        //{
        //    Point pt = clientToScreen(e.Location);
        //    foreach (Button btn in m_btns)
        //    {
        //        if (btn.Bounds.Contains(pt))
        //        {
        //            btn.PerformClick();
        //            break;
        //        }
        //    }
        //}
    }
}
