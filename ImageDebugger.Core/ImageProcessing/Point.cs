using System;
using System.Collections.Generic;
using HalconDotNet;

namespace ImageDebugger.Core.ImageProcessing
{
    public class Point
    {
        public static List<HObject> GraphicPoints { get; set; } = new List<HObject>();

        public static int PointSize { get; set; } = 100;

        public static double PointAngle { get; set; } = 0.8;

        public double ImageX { get; set; }

        public double ImageY { get; set; }

        public double CoordinateX { get; set; }

        public double CoordinateY { get; set; }

        public void Display(HWindow windowHandle)
        {
            foreach (var graphicPoint in GraphicPoints)
            {
                graphicPoint.DispObj(windowHandle);
            }

            GraphicPoints.Clear();
        }

        public Point(double imageX, double imageY, bool display = false)
        {
            ImageX = imageX;
            ImageY = imageY;
            if (display)
            {
                HObject cross;
                HOperatorSet.GenCrossContourXld(out cross, imageY, imageX, PointSize, PointAngle);
                GraphicPoints.Add(cross);
            }
        }

        /// <summary>
        /// Return the average point of two points in image
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Point CenterPointInImage(Point pt1, Point pt2)
        {
            return new Point((pt1.ImageX + pt2.ImageX) / 2.0, (pt1.ImageY + pt2.ImageY)/2.0);
        }

        /// <summary>
        /// Compute direct point point distance
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static double Distance(Point pt1, Point pt2)
        {
            return Math.Sqrt((pt1.ImageX - pt2.ImageX) * (pt1.ImageX - pt2.ImageX) +
                             (pt1.ImageY - pt2.ImageY) * (pt1.ImageY - pt2.ImageY));
        }
    }
}