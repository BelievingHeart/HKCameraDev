using System;
using System.Collections.Generic;
using HalconDotNet;
using UI.ImageProcessing;
using UI.ImageProcessing.Utilts;

namespace UI.Models
{
    public class HalconGraphics
    {
        public List<Tuple<List<double>, List<double>>> CrossesUsed { get; set; }
        public HObject CrossesIgnored { get; set; }
        public List<Line> PointLineGraphics { get; set; }
        public List<Line> PointPointGraphics { get; set; }
        public HObject FindLineRects { get; set; }
        public HObject LineRegions { get; set; }

        public HImage Image { get; set; }

        
        private static readonly HDevelopExport HalconScripts = new HDevelopExport();

        public void DisplayGraphics(HWindow windowHandle)
        {

            
            windowHandle.DispImage(Image);
            DisplayCrosses(windowHandle);
            DisplayPointLineDistanceGraphics(windowHandle);
            DisplayPointPointDistanceGraphics(windowHandle);
            DisplayEdges(windowHandle);
            Line.DisplayGraphics(windowHandle);
        }

        private void DisplayEdges(HWindow windowHandle)
        {
            if (Edges == null) return;
            windowHandle.SetLineWidth(5);
            windowHandle.SetColor("orange");
            Edges.DispObj(windowHandle);
            windowHandle.SetLineWidth(1);
        }


        public int ImageHeight { get; set; } = 5120;

        public int ImageWidth { get; set; } = 5120;


        private void DisplayPointLineDistanceGraphics(HWindow windowHandle)
        {
            windowHandle.SetColor("yellow");
            windowHandle.SetLineWidth(3);
            if (PointLineGraphics == null) return;
            foreach (var line in PointLineGraphics)
            {
                windowHandle.DispArrow(line.YStart, line.XStart, line.YEnd, line.XEnd, ArrowSize);
            }
            windowHandle.SetLineWidth(1);

        }
        private void DisplayPointPointDistanceGraphics(HWindow windowHandle)
        {
            windowHandle.SetColor("orange");
            windowHandle.SetDraw("fill");

            if (PointLineGraphics == null) return;

            HObject draw = new HObject();
            draw.GenEmptyObj();
            foreach (var line in PointPointGraphics)
            {
                HObject circle1, circle2, lineSeg;
                HOperatorSet.GenCircle(out circle1, line.YStart, line.XStart, EndPointRadius);
                HOperatorSet.GenCircle(out circle2, line.YEnd, line.XEnd, EndPointRadius);
                HOperatorSet.GenRegionLine(out lineSeg, line.YStart, line.XStart, line.YEnd, line.XEnd);
                draw = HalconHelper.ConcatAll(draw, circle1, circle2, lineSeg);
            }

            windowHandle.DispObj(draw);
            windowHandle.SetDraw("margin");
        }

        public int EndPointRadius { get; set; } = 10;

        public double ArrowSize { get; set; } = 10;
        public HObject Edges { get; set; }

        private void DisplayCrosses(HWindow windowHandle)
        {

            windowHandle.SetDraw("margin");
            windowHandle.SetLineWidth(1);
            if (CrossesUsed != null && CrossesUsed.Count > 0)
            {
                windowHandle.SetColor("green");
                HObject crossesAllLine = new HObject();
                crossesAllLine.GenEmptyObj();
                foreach (var tuple in CrossesUsed)
                {
                    var xs = tuple.Item1.ToArray();
                    var ys = tuple.Item2.ToArray();
                    HObject crossesOneLine;
                    HOperatorSet.GenCrossContourXld(out crossesOneLine, ys, xs, 0.5, 0.5);
                    crossesAllLine = crossesAllLine.ConcatObj(crossesOneLine);
                }
                windowHandle.DispObj(crossesAllLine);
            }
            

            windowHandle.SetColor("red");
            CrossesIgnored?.DispObj(windowHandle);

            windowHandle.SetColor("magenta");
            windowHandle.SetLineWidth(3);
            FindLineRects?.DispObj(windowHandle);

            
            windowHandle.SetColor("blue");
            LineRegions?.DispObj(windowHandle);
        }
    }
}