using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace WacomWebSocketService.Entities
{

    //This entity stores the graphometric signature from Wacom Pad
    public class GraphSign
    {
        private int maxOrder;
        private Bitmap image;
        private int pointsWithPressure;

        public int PointsWithPressure
        {
            get { return pointsWithPressure; }
            set { pointsWithPressure = value; }
        }
        public Bitmap Image
        {
            get { return image; }
            set { image = value; }
        }
        private List<BioSignPoint> points;

        internal List<BioSignPoint> Points
        {
            get { return points; }
            set { points = value; }
        }

        public GraphSign()
        {
            this.points = new List<BioSignPoint>();
            this.maxOrder = 0;
            this.pointsWithPressure = 0;
        }

        public void addPoint(BioSignPoint point)
        {
            point.Order = this.maxOrder;
            maxOrder++;
            if (point.Pressure > 0)
            {
                this.pointsWithPressure++;
            }
            this.points.Add(point);
        }

        internal void ClearPoints()
        {
            this.points.Clear();
            this.maxOrder = 0;
            this.pointsWithPressure = 0;
        }
    }
}
