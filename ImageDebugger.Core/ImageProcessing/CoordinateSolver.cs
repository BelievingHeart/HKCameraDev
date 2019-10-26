using System.Collections.Generic;
using HalconDotNet;
using ImageDebugger.Core.Models;

namespace ImageDebugger.Core.ImageProcessing
{
    public class CoordinateSolver
    {
        private HTuple _changeOfBase, _changeOfBaseInv, _rotationMat, _rotationMatInv, _mapToWorld, _mapToImage;
        private static HDevelopExport HalconScripts = new HDevelopExport();
        public List<Line> PointLineDistanceGraphics { get; } = new List<Line>();
        public List<Line> PointPointDistanceGraphics { get; } = new List<Line>();

        public CoordinateSolver(HTuple changeOfBase, HTuple changeOfBaseInv, HTuple rotationMat, HTuple rotationMatInv,
            HTuple mapToWorld, HTuple mapToImage)
        {
            _changeOfBase = changeOfBase;
            _changeOfBaseInv = changeOfBaseInv;
            _rotationMat = rotationMat;
            _rotationMatInv = rotationMatInv;
            _mapToWorld = mapToWorld;
            _mapToImage = mapToImage;
        }

        /// <summary>
        /// Calculate absolute angle in Halcon's representation from an angle of normal representation'
        /// </summary>
        /// <param name="angleNormal"></param>
        /// <returns></returns>
        public double DegreeRelativeToAbsolute(double angleNormal)
        {
            HTuple degreeAbs;
            HalconScripts.DegreeRelativeToAbs(_rotationMat, angleNormal, out degreeAbs);

            return degreeAbs.D;
        }

        public double PointLineDistanceInWorld(double x, double y, Line line, bool display = true)
        {
            HTuple distanceWorld, distancePixel;
            HalconScripts.DistancePLInWorld(line.XStart, line.YStart, line.XEnd, line.YEnd, x, y, _mapToWorld,
                out distanceWorld, out distancePixel);

            if (display)
            {
                HTuple xIntersect, yIntersect;
                HalconScripts.get_perpendicular_line_that_passes(line.XStart, line.YStart, line.XEnd, line.YEnd, x, y,
                    out xIntersect, out yIntersect);
                PointLineDistanceGraphics.Add(new Line(x, y, xIntersect.D, yIntersect.D));
            }

            return distanceWorld.D;
        }
        

      

        public double EndPointRadius { get; set; } = 10;

 

        public static double ArrowSize { get; set; } = 10;

        /// <summary>
        /// Translate a line at a distance measured in millimeter
        /// </summary>
        /// <param name="distance">mm distance</param>
        /// <param name="line"></param>
        /// <param name="display">whether to push the line to a static graphic for displaying</param>
        /// <returns></returns>
        public Line TranslateLineInWorldUnit(double distance, Line line, bool display = false)
        {
            HTuple startX, startY, endX, endY;
            HalconScripts.TranslateLineInWorldCoordinateAndConvertBack(line.XStart, line.YStart, line.XEnd, line.YEnd,
                distance, _mapToWorld, _mapToImage, line.IsVertical ? "true" : "false", out startX, out startY,
                out endX, out endY);

            return new Line(startX.D, startY.D, endX.D, endY.D, display);
        }

        /// <summary>
        /// Calculate an absolute point from a relative point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point PointRelativeToAbsolute(double x, double y)
        {
            HTuple xAbs;
            HTuple yAbs;
            HalconScripts.PointRelativeToAbs(x, y, _changeOfBase, out xAbs, out yAbs);
            return new Point(xAbs.D, yAbs.D);
        }

        public FindLineLocation FindLineLocationRelativeToAbsolute(FindLineLocation relativeLocation)
        {
            var point = PointRelativeToAbsolute(relativeLocation.X, relativeLocation.Y);
            var angle = DegreeRelativeToAbsolute(relativeLocation.Angle);

            return new FindLineLocation()
            {
                Angle = angle, Len1 = relativeLocation.Len1, Len2 = relativeLocation.Len2, X = point.ImageX, Y = point.ImageY,
                ImageIndex = relativeLocation.ImageIndex, Name = relativeLocation.Name
            };
        }
        
        public double PointLineDistanceInWorld(Point point, Line line, bool display = true)
        {
            return PointLineDistanceInWorld(point.ImageX, point.ImageY, line, display);
        }

        public double PointPointDistanceInWorld(Point pointA, Point pointB, bool display = true)
        {
            HTuple distance;
            HalconScripts.DistanceInWorld_PP(pointA.ImageY, pointA.ImageX, pointB.ImageY, pointB.ImageX, _mapToWorld, out distance);
            if (display)
            {
                PointPointDistanceGraphics.Add(new Line(pointA.ImageX, pointA.ImageY, pointB.ImageX, pointB.ImageY));
            }

            return distance.D;
        }
        
    }
}