using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Models.Regression.Linear;
using HalconDotNet;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Statistics;



namespace ImageDebugger.Core.ImageProcessing.Utilts
{
    public static class HalconHelper
    {
        private static HDevelopExport HalconScripts = new HDevelopExport();

        public static HObject ConcatAll(params HObject[] objects)
        {
            var objectOut = objects[0];
            for (int i = 1; i < objects.Length; i++)
            {
                HOperatorSet.ConcatObj(objectOut, objects[i], out objectOut);
            }

            return objectOut;
        }
        
        
        public static HImage HobjectToHimage(this HObject hobject) 
        { 
            HImage image = new HImage();
            HTuple pointer, type, width, height; 
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height); 
            image.GenImage1(type, width, height, pointer);
            return image;
        } 

        public static Line FitLine2D(List<double> xs, List<double> ys)
        {
            var yDeviation = ys.StandardDeviation();
            var xDeviation = xs.StandardDeviation();
            var isVertical = yDeviation > xDeviation;

            if(isVertical) Swap(ref xs, ref ys);

            var biasAndWeight = SimpleRegression.Fit(xs.ToArray(), ys.ToArray());
            var bias = biasAndWeight.Item1;
            var weight = biasAndWeight.Item2;

            var xStart = xs.Min();
            var xEnd = xs.Max();

            var yStart = xStart * weight + bias;
            var yEnd = xEnd * weight + bias;

          

            return new Line(isVertical?yStart: xStart, isVertical ?xStart: yStart, isVertical ? yEnd: xEnd, isVertical ? xEnd: yEnd);
        }

        private static void Swap(ref List<double> xs, ref List<double> ys)
        {
            var tempY = new List<double>();
            foreach (var x in xs)
            {
                tempY.Add(x);
            }

            var tempX = new List<double>();
            foreach (var y in ys)
            {
                tempX.Add(y);
            }

            xs = tempX;
            ys = tempY;
        }

        public static Tuple<List<double>, List<double>> FindLineSubPixel(HImage image, double[] row, double[] col, double[] radian,
            double[] len1, double[] len2, string transition, int numSubRects, int threshold, string whichEdge, double ignoreFraction, int cannyLow, int cannyHigh, double sigma1, double sigma2,
            int newWidth, int kernelWidth,HTuple longestOnly, out HObject edges, out HObject findLineRects)
        {
            var length = row.Length;
            findLineRects = new HObject();
            findLineRects.GenEmptyObj();
            edges = new HObject();
            edges.GenEmptyObj();

            var outputXs = new List<double>();
            var outputYs = new List<double>();

            // For each find line rect
            for (int i = 0; i < length; i++)
            {
                HObject findLineRect, edge;
                HTuple xs, ys, _;
                HalconScripts.VisionProStyleFindLine(image, out findLineRect, transition, row[i], col[i], radian[i],
                    len1[i], len2[i], numSubRects, threshold, sigma1, whichEdge, "false", "first", 0, 0, out xs, out ys);


                var lineOnEdge = FitLine2D(xs.DArr.ToList(), ys.DArr.ToList());
                

                HalconScripts.GetEdgesInSubRect2(image, out findLineRect, out edge, lineOnEdge.XStart, lineOnEdge.YStart, lineOnEdge.XEnd, lineOnEdge.YEnd, radian[i], newWidth, sigma2, cannyLow, cannyHigh, kernelWidth, longestOnly);
                edges.ConcatObj(edge);
                edges = ConcatAll(edges, edge);
                findLineRects = ConcatAll(findLineRects, findLineRect);


                List<double> contourXs, contourYs;
                GetContoursPoints(edge, out contourXs, out contourYs);
                outputXs.AddRange(contourXs);
                outputYs.AddRange(contourYs);
            }

            return new Tuple<List<double>, List<double>>(outputXs, outputYs);
        }

        public static void GetContoursPoints(HObject edges, out List<double> contourXs, out List<double> contourYs)
        {
            contourXs = new List<double>();
            contourYs = new List<double>();

            int edgeCount = edges.CountObj();

            for (int i = 1; i < edgeCount + 1; i++)
            {
                HTuple ys, xs;
                HOperatorSet.GetContourXld(edges[i], out ys, out xs);
                contourXs.AddRange(xs.ToDArr());
                contourYs.AddRange(ys.ToDArr());
            }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Fit line with Ransac algorithm
        /// </summary>
        /// <param name="xs">x components of input points</param>
        /// <param name="ys">y components of input points</param>
        /// <param name="errorThreshold"></param>
        /// <param name="maxTrials"></param>
        /// <param name="ignoreFraction"></param>
        /// <param name="probability"></param>
        /// <param name="xsUsed">x components of the inlier points</param>
        /// <param name="ysUsed">y components of the inlier points</param>
        /// <returns>
        /// Fitted line with the farthest end points from inlier points
        /// </returns>
        public static Line RansacFitLine(double[] xs, double[] ys, double errorThreshold, int maxTrials, double ignoreFraction, double probability, out IEnumerable<double> xsUsed, out IEnumerable<double> ysUsed)
        {

            var xDeviation = xs.StandardDeviation();
            var yDeviation = ys.StandardDeviation();
            var isVertical = yDeviation > xDeviation;
            if (isVertical) Swap(ref xs, ref ys);

            // Now, fit simple linear regression using RANSAC
            int minSamples = (int) (xs.Length * (1 - ignoreFraction));
          

            // Create a RANSAC algorithm to fit a simple linear regression
            var ransac = new RANSAC<SimpleLinearRegression>(minSamples)
            {
                Probability = probability,
                Threshold = errorThreshold,
                MaxEvaluations = maxTrials,

                // Define a fitting function
                Fitting = (int[] sample) =>
                {
                    // Build a Simple Linear Regression model
                    return new OrdinaryLeastSquares()
                        .Learn(xs.Get(sample), ys.Get(sample));
                },

                // Define a inlier detector function
                Distances = (SimpleLinearRegression r, double threshold) =>
                {
                    var inliers = new List<int>();
                    for (int i = 0; i < xs.Length; i++)
                    {
                        // Compute error for each point
                        double error = r.Transform(xs[i]) - ys[i];

                        // If the square error is low enough,
                        if (error * error < threshold)
                            inliers.Add(i); //  the point is considered an inlier.
                    }

                    return inliers.ToArray();
                }
            };


            // Now that the RANSAC hyperparameters have been specified, we can 
            // compute another regression model using the RANSAC algorithm:

            int[] inlierIndices;
            SimpleLinearRegression robustRegression = ransac.Compute(xs.Length, out inlierIndices);

            var weight = robustRegression.Slope;
            var bias = robustRegression.Intercept;

            var xStart = xs.Min();
            var xEnd = xs.Max();

            var yStart = xStart * weight + bias;
            var yEnd = xEnd * weight + bias;


            xsUsed = xs.SelectByIndices(inlierIndices);
            ysUsed = ys.SelectByIndices(inlierIndices);
            if (isVertical) Swap(ref xsUsed, ref ysUsed);

            return new Line(isVertical ? yStart : xStart, isVertical ? xStart : yStart, isVertical ? yEnd : xEnd, isVertical ? xEnd : yEnd);
        }


        public static IEnumerable<double> SelectByIndices(this IEnumerable<double> source, IEnumerable<int> indices)
        {
            var output = new List<double>();
            foreach (var index in indices)
            {
                output.Add(source.ElementAt(index));
            }

            return output;
        }



        #region Chu's line fitting

        /// <summary>
        /// 根据点坐标拟合一条直线
        /// </summary>
        public static Line leastSquareAdaptLine(List<double> xArray, List<double> yArray)
        {
            Trace.Assert(xArray.Count == yArray.Count);
            cylineParam lineValue = new cylineParam() { A = 0, B = 0, C = 0 };

            var xStart = xArray.Min();
            var xEnd = xArray.Max();

            int nums = xArray.Count;
            List<double> matrix = new List<double>() { 0, 0, 0, 0 };
            List<double> bias = new List<double>() { 0, 0 };
            double xx = 0, xy = 0, yy = 0, x = 0, y = 0;
            for (int i = 0; i < xArray.Count; i++)
            {
                xx += xArray[i] * xArray[i];
                yy += yArray[i] * yArray[i];
                x += xArray[i];
                xy += xArray[i] * yArray[i];
                y += yArray[i];
            }
            double dVAlue1 = xx - (x * x) / nums;
            double dValue2 = yy - (y * y) / nums;
            bool bFunX = true;
            // y方向的方差更小， 那证明， 应该采用 x= a*y +b;
            if (dVAlue1 < dValue2)
                bFunX = false;

            if (bFunX == true)
            {
                //y = ax+b;
                matrix[0] = xx; matrix[1] = x;
                matrix[2] = matrix[1]; matrix[3] = nums;
                bias[0] = xy; bias[1] = y;
            }
            else
            {
                //ay +b = x
                matrix[0] = yy; matrix[1] = y;
                matrix[2] = matrix[1]; matrix[3] = nums;
                bias[0] = xy; bias[1] = x;
            }

            List<double> Invmatrix = null;
            MatrixInv2X2(matrix, out Invmatrix);

            // AAd*y = a*x +c
            List<double> reusltMatrix = new List<double>();
            Matrix_Mult2X1(Invmatrix, bias, out reusltMatrix);
            double a = reusltMatrix[0];
            double c = reusltMatrix[1];

            //三次去掉最大的偏差点
            List<double> listDist = new List<double>();
    


            int nums2 = xArray.Count();
            for (int t = 0; t < 3; t++)
            {
                // 求平均距离， 和最大的偏置项，在置信度[-99.6, 99.6]内的值保留，其他的去掉
                double sValue = 0;
                double uValue = 0;
                if (xArray.Count() < nums2 * 2 / 3 || xArray.Count() <= 2)
                    break;
                for (int j = 0; j < xArray.Count; j++)
                {
                    double pValue = 0;
                    pValue = (bFunX == true) ? (xArray[j] * a + c - yArray[j]) / Math.Sqrt(a * a + 1) :
                                                (yArray[j] * a + c - xArray[j]) / Math.Sqrt(a * a + 1);
                    listDist.Add(pValue);
                    uValue += pValue;
                    sValue += pValue * pValue;
                }
                uValue /= xArray.Count;
                sValue = Math.Sqrt((sValue - (uValue * uValue) * xArray.Count) / (xArray.Count - 1));

                double distMin = uValue - 1.96 * sValue;
                double distMax = uValue + 1.96 * sValue;
                nums = xArray.Count;
                for (int j = 0; j < xArray.Count; j++)
                {
                    if (listDist[j] > distMin && listDist[j] < distMax)
                        continue;

                    matrix[0] -= ((bFunX == true) ? (xArray[j] * xArray[j]) : (yArray[j] * yArray[j]));
                    matrix[1] -= ((bFunX == true) ? xArray[j] : yArray[j]);
                    bias[0] -= xArray[j] * yArray[j];
                    bias[1] -= ((bFunX == true) ? yArray[j] : xArray[j]);

                    xArray.RemoveAt(j);
                    yArray.RemoveAt(j);
                    listDist.RemoveAt(j);

                    //double xValue = xArray[j];
                    //double yValue = yArray[j];
                    //moveXArray.Add(xValue);
                    //moveYArray.Add(yValue);
                    j--;
                }
                matrix[3] = xArray.Count;
                matrix[2] = matrix[1];
                MatrixInv2X2(matrix, out Invmatrix);
                Matrix_Mult2X1(Invmatrix, bias, out reusltMatrix);
                a = reusltMatrix[0];
                c = reusltMatrix[1];
                listDist.Clear();
                if (xArray.Count == nums)
                    break;
            }

            double div = Math.Sqrt(a * a + 1);
            if (bFunX == true)
            {
                // y=ax+c
                lineValue.A = a / div;
                lineValue.B = -1 / div;
                lineValue.C = -c / div;
            }
            else
            {
                //x = ay+c
                lineValue.A = 1 / div;
                lineValue.B = -a / div;
                lineValue.C = c / div;
            }

            var yStart = (lineValue.C - lineValue.A * xStart) / lineValue.B;
            var yEnd = (lineValue.C - lineValue.A * xEnd) / lineValue.B;

            return new Line(xStart, yStart, xEnd, yEnd);
        }

        public static void MatrixInv2X2(List<double> Rmatrix, out List<double> RInvmatrix)
        {
            RInvmatrix = new List<double>() { 0, 0, 0, 0 };
            double div = Rmatrix[3] * Rmatrix[0] - Rmatrix[1] * Rmatrix[2];
            if (Math.Abs(div) <= 1e-24)
                return;
            RInvmatrix[0] = Rmatrix[3] / div; RInvmatrix[1] = -Rmatrix[1] / div;
            RInvmatrix[2] = -Rmatrix[2] / div; RInvmatrix[3] = Rmatrix[0] / div;
        }

     
        public static void Matrix_Mult2X1(List<double> Rmatrix, List<double> input, out List<double> result)
        {
            result = new List<double>() {0, 0};
            if (Rmatrix.Count != 4)
                return;
            if (input.Count != 2)
                return;

            result[0] = Rmatrix[0] * input[0] + Rmatrix[1] * input[1];
            result[1] = Rmatrix[2] * input[0] + Rmatrix[3] * input[1];
        }

        #endregion
    }

    /// <summary>
    /// 直线参数
    /// </summary>
    public struct cylineParam
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public static cylineParam operator *(cylineParam a, double b)
        {
            return new cylineParam() { A = a.A * b, B = a.B * b, C = a.C * b };
        }
    }
}