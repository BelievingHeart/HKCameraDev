using HalconDotNet;
using ImageDebugger.Core.Models;

namespace ImageDebugger.Core.ImageProcessing.BottomView
{
    public partial class I94BottomViewMeasure : IMeasurementProcedure
    {
        public string Name { get; } = "I94_BOTTOM";

        private readonly HDevelopExport HalconScripts = new HDevelopExport();
        private HTuple _shapeModelHandle;

     

        public double Weight { get; set; } = 0.0076;


        public I94BottomViewMeasure()
        {
            HOperatorSet.ReadShapeModel("./backViewModel", out _shapeModelHandle);
        }

        /// <summary>
        /// Get the longest contour with in a region
        /// </summary>
        /// <param name="image"></param>
        /// <param name="location"></param>
        /// <param name="cannyLow"></param>
        /// <param name="cannyHigh"></param>
        /// <returns></returns>
        private HObject GetContour(HImage image, FindLineLocation location, int cannyLow = 20, int cannyHigh = 40)
        {
            HObject region;
            HOperatorSet.GenRectangle2(out region, location.Y, location.X, MathUtils.ToRadian(location.Angle),
                location.Len1, location.Len2);
            var imageEdge = image.ReduceDomain(new HRegion(region));
            return imageEdge.EdgesSubPix("canny", 3, cannyLow, cannyHigh);
        }

        /// <summary>
        /// Return a single intersection point of a line and a contour
        /// </summary>
        /// <param name="line"></param>
        /// <param name="contour"></param>
        /// <returns></returns>
        private Point LineContourIntersection(Line line, HObject contour)
        {
            HTuple x, y, _, contourLength;
            HalconScripts.LongestXLD(contour, out contour, out contourLength);
            HOperatorSet.IntersectionLineContourXld(contour, line.YStart, line.XStart, line.YEnd, line.XEnd, out y,
                out x, out _);

            return new Point(x.D, y.D);
        }
    }
}