using System;
using System.Collections.Generic;
using System.IO;
using HalconDotNet;

namespace ImageDebugger.Core.ImageProcessing
{
    public class DataRecorder
    {
        private HTuple _changeOfBaseInv;

        private static HDevelopExport HalconScripts = new HDevelopExport();

        public static Point Offset { get; set; } = new Point(10, 0);

        private Dictionary<string, Point> _points = new Dictionary<string, Point>();
        private Dictionary<string, double> _angles = new Dictionary<string, double>();

        public DataRecorder(HTuple changeOfBaseInv)
        {
            _changeOfBaseInv = changeOfBaseInv;
        }

        /// <summary>
        /// Record a point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="name"></param>
        public void RecordPoint(Point point, string name)
        {
            AssignCoordinatePoint(point);
            _points[name] = point;
        }

        /// <summary>
        /// Record a point from intersecting lines
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="name"></param>
        public void RecordPoint(Line lineA, Line lineB, string name)
        {
            var point = lineA.Intersect(lineB);
            RecordPoint(point, name);
        }

        public void RecordLine(Line line, string name)
        {
            _angles[name] = line.Angle;
        }

        private void AssignCoordinatePoint(Point point)
        {
            HTuple xOut, yOut;
            HOperatorSet.AffineTransPoint2d(_changeOfBaseInv, point.ImageX, point.ImageY, out xOut, out yOut);
            point.CoordinateX = xOut;
            point.CoordinateY = yOut;
        }

        public void DisplayPoints(HWindow windowHandle)
        {
            foreach (var pair in _points)
            {
                var point = pair.Value;
                windowHandle.DispText($"({point.CoordinateX.ToString("f2")}, {point.CoordinateY.ToString("f3")})", "image", point.ImageY + Offset.ImageY, point.ImageX + Offset.ImageX, "red", "border_radius", 2);
            }
        }


        public void Serialize(string path)
        {

            var dir = Directory.GetParent(path).FullName;
            Directory.CreateDirectory(dir);
            // Create header line
            var headerNames = new List<string>();
            // Add in point header
            foreach (var pair in _points)
            {
                var xName = pair.Key + "_X";
                var yName = pair.Key + "_Y";
                headerNames.Add(xName);
                headerNames.Add(yName);
            }
            // Add in angle header
            headerNames.Add(" ");
            foreach (var angle in _angles)
            {
                headerNames.Add("angle_" + angle.Key);
            }
            var header = string.Join(",", headerNames);
            
            
            // Create value line
            var valueStrings = new List<string>();
            // Add in point values
            foreach (var pair in _points)
            {
                var xValue = pair.Value.CoordinateX.ToString("f3");
                var yValue = pair.Value.CoordinateY.ToString("f3");
                valueStrings.Add(xValue);
                valueStrings.Add(yValue);
            }
            // Add in angle values
            valueStrings.Add(" ");
            foreach (var angle in _angles)
            {
                valueStrings.Add(angle.Value.ToString("f4"));
            }

            var line = string.Join(",", valueStrings);

            // Write to file
            var fileExists = File.Exists(path);
            var lineToWrite = fileExists ? line : header + Environment.NewLine + line;
            using (var fs = new StreamWriter(path, fileExists))
            {
                fs.WriteLine(lineToWrite);
            }
        }
    }


}