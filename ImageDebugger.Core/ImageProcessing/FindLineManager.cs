using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HalconDotNet;
using ImageDebugger.Core.ImageProcessing.Utilts;
using MaterialDesignThemes.Wpf;
using MathNet.Numerics.LinearRegression;

namespace ImageDebugger.Core.ImageProcessing
{
    //Conventions:
    // 1. All the someNumber-someLetter with the same someNumber will come to a single line result, whose name is someNumber
    // 2. All the someNumber.someLetter with the same someNumber will result in individual lines, whose name is someNumber.someLetter

    /// <summary>
    /// 
    /// </summary>
    public class FindLineManager
    {
        public List<Tuple<List<double>, List<double>>> CrossesUsed
        {
            get { return _crossesUsed; }
            set { _crossesUsed = value; }
        }

        public HObject CrossesIgnored
        {
            get { return _crossesIgnored; }
            set { _crossesIgnored = value; }
        }

        public HObject LineRegions
        {
            get { return _lineRegions; }
            set { _lineRegions = value; }
        }

        public HObject FindLineRects
        {
            get { return _findLineRects; }
            set { _findLineRects = value; }
        }

        public SnackbarMessageQueue MessageQueue { get; set; }


        private Dictionary<string, Line> _lines = new Dictionary<string, Line>();

        private int _width = 5120;
        private int _height = 5120;
        private HObject _crossesIgnored = new HObject();
        private List<Tuple<List<double>, List<double>>> _crossesUsed = new List<Tuple<List<double>, List<double>>>();
        private HObject _findLineRects = new HObject();
        private HObject _lineRegions = new HObject();

        private static HDevelopExport HalconScripts = new HDevelopExport();
        private HObject _edges = new HObject();


        private void PromptUserInvoke(string message)
        {
            MessageQueue?.Enqueue(message); 
        }

        public void FindLines(List<HImage> images)
        {
            var imageFeedingDict = AssociateFeedingsWithImages(images);
            foreach (var pair in imageFeedingDict)
            {
                var name = pair.Key;
                var image = pair.Value.Item1;
                var feeding = pair.Value.Item2;
                Line line = TryFindLine(name, image, feeding);
                _lines[name] = line;
            }
        }

        public Line TryFindLine(string name, HImage image, FindLineFeeding feeding)
        {
            Line line = new Line();
            try
            {
                line = FindLine(image, feeding);
            }
            catch (Exception e)
            {
                PromptUserInvoke($"Line {name} not found! {Environment.NewLine} {e.Message}");
            }

            return line;
        }

        private Dictionary<string, Tuple<HImage, FindLineFeeding>> AssociateFeedingsWithImages(List<HImage> images)
        {
            var output = new Dictionary<string, Tuple<HImage, FindLineFeeding>>();
            foreach (var pair in FindLineFeedings)
            {
                var name = pair.Key;
                var feeding = pair.Value;
                var image = images[feeding.ImageIndex];
                output[name] = new Tuple<HImage, FindLineFeeding>(image, feeding);
            }

            return output;
        }

        public async Task FindLinesParallel(List<HImage> images)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var imageFeedingDict = AssociateFeedingsWithImages(images);

            var findLineTasks = new List<Task>();
            foreach (var pair in imageFeedingDict)
            {
                findLineTasks.Add(Task.Run(() =>
                {
                    // find line
                    var name = pair.Key;
                    var image = pair.Value.Item1;
                    var feeding = pair.Value.Item2;
                    var line = TryFindLine(name, image, feeding);

                    // Record line
                    _lines[name] = line;
                }));
            }

            await Task.WhenAll(findLineTasks);
            stopwatch.Stop();

            var timeElapse = stopwatch.ElapsedMilliseconds;
        }

        public Dictionary<string, FindLineFeeding> FindLineFeedings { get; set; }

        public Line FindLine(HImage image, FindLineFeeding feeding)
        {
            HObject lineRegion, findLineRegion;
            HTuple xsUsed = new HTuple();
            HTuple ysUsed = new HTuple();
            HTuple xsIgnored = new HTuple();
            HTuple ysIgnored = new HTuple();
            HTuple lineX1 = new HTuple();
            HTuple lineY1 = new HTuple();
            HTuple lineX2 = new HTuple();
            HTuple lineY2 = new HTuple();
            List<double> ys, xs;


            // using pair
            HObject edges = new HObject();
            edges.GenEmptyObj();
            if (feeding.UsingPair)
            {
                if (feeding.FirstAttemptOnly)
                {
                    HalconScripts.FindLineGradiant_Pair(image, out findLineRegion, out lineRegion, feeding.Row,
                        feeding.Col, feeding.Radian, feeding.Len1, feeding.Len2, feeding.NumSubRects,
                        feeding.IgnoreFraction, feeding.Transition, feeding.Threshold, feeding.Sigma1,
                        feeding.WhichEdge, feeding.WhichPair, feeding.MinWidth, feeding.MaxWidth, out xsUsed,
                        out ysUsed, out xsIgnored, out ysIgnored, out lineX1, out lineY1, out lineX2, out lineY2);
                }
                else
                {
                    HalconScripts.VisionProStyleFindLineOneStep_Pairs(image, out findLineRegion, out lineRegion,
                        feeding.Row, feeding.Col, feeding.Radian, feeding.Len1, feeding.Len2, feeding.Transition,
                        feeding.NumSubRects, feeding.Threshold, feeding.Sigma1, feeding.Sigma2, feeding.WhichEdge,
                        feeding.IsVertical, feeding.IgnoreFraction, feeding.WhichPair, feeding.MinWidth,
                        feeding.MaxWidth, _width, _height, feeding.CannyHigh, feeding.CannyLow, "true",
                        feeding.NewWidth, feeding.KernelWidth,feeding.LongestOnly, out xsUsed, out ysUsed, out xsIgnored, out ysIgnored,
                        out lineX1, out lineY1,
                        out lineX2, out lineY2);
                }

                xs = xsUsed.DArr.ToList();
                ys = ysUsed.DArr.ToList();
            } // using single edge
            else
            {
                if (feeding.FirstAttemptOnly)
                {
                    HalconScripts.FindLineGradiant(image, out findLineRegion, out lineRegion, feeding.Row, feeding.Col,
                        feeding.Radian, feeding.Len1, feeding.Len2, feeding.NumSubRects, feeding.IgnoreFraction,
                        feeding.Transition, feeding.Threshold, feeding.Sigma1, feeding.WhichEdge, out xsUsed,
                        out ysUsed, out xsIgnored, out ysIgnored, out lineX1, out lineY1, out lineX2, out lineY2);
                    ys = ysUsed.DArr.ToList();
                    xs = xsUsed.DArr.ToList();
                }
                else

                {
//                    HalconScripts.VisionProStyleFindLineOneStep(image, out findLineRegion, out lineRegion,
//                        feeding.Transition, feeding.Row, feeding.Col, feeding.Radian, feeding.Len1, feeding.Len2,
//                        feeding.NumSubRects, feeding.Threshold, feeding.WhichEdge, feeding.IgnoreFraction,
//                        feeding.IsVertical, feeding.Sigma1, feeding.Sigma2, _width, _height, feeding.NewWidth,
//                        feeding.CannyHigh, feeding.CannyLow, out lineX1, out lineY1, out lineX2, out lineY2, out xsUsed,
//                        out ysUsed, out xsIgnored, out ysIgnored);

                    var xsys = HalconHelper.FindLineSubPixel(image, feeding.Row.DArr, feeding.Col.DArr,
                        feeding.Radian.DArr, feeding.Len1.DArr, feeding.Len2.DArr, feeding.Transition.S,
                        feeding.NumSubRects.I, feeding.Threshold.I, feeding.WhichEdge.S, feeding.IgnoreFraction.D,
                        feeding.CannyLow.I, feeding.CannyHigh.I, feeding.Sigma1.D, feeding.Sigma2.D, feeding.NewWidth.I,
                        feeding.KernelWidth, feeding.LongestOnly,
                        out edges, out findLineRegion);

                    xs = xsys.Item1;
                    ys = xsys.Item2;
                }
            }


            IEnumerable<double> xsInlier, ysInlier;
            var line = HalconHelper.RansacFitLine(xs.ToArray(), ys.ToArray(), feeding.ErrorThreshold, feeding.MaxTrials,
                feeding.IgnoreFraction, feeding.Probability, out xsInlier, out ysInlier);
            xs = xsInlier.ToList();
            ys = ysInlier.ToList();


            HalconScripts.GenLineRegion(out lineRegion, line.XStart, line.YStart, line.XEnd, line.YEnd, _width,
                _height);
            lineX1 = line.XStart;
            lineY1 = line.YStart;
            lineX2 = line.XEnd;
            lineY2 = line.YEnd;


            // Generate debugging graphics 

            HObject crossesIgnored;
            HOperatorSet.GenCrossContourXld(out crossesIgnored, ysIgnored, xsIgnored, CrossSize, CrossAngle);

            // Critical section
            lock (this)
            {
                CrossesUsed.Add(new Tuple<List<double>, List<double>>(xs, ys));
                HOperatorSet.ConcatObj(_crossesIgnored, crossesIgnored, out _crossesIgnored);
                HOperatorSet.ConcatObj(_findLineRects, findLineRegion, out _findLineRects);
                HOperatorSet.ConcatObj(_lineRegions, lineRegion, out _lineRegions);
                HOperatorSet.ConcatObj(Edges, edges, out _edges);
            }


            return new Line(lineX1.D, lineY1.D, lineX2.D, lineY2.D);
        }

        private void FitLineRegression(double[] xs, double[] ys, out HTuple lineX1, out HTuple lineY1,
            out HTuple lineX2, out HTuple lineY2)
        {
            var lineResult = SimpleRegression.Fit(xs, ys);
            var bias = lineResult.Item1;
            var weight = lineResult.Item2;
            HalconScripts.ImageLineIntersections(weight, bias, _width, _height, out lineX1, out lineY1, out lineX2,
                out lineY2);
        }


        public Line GetLine(string lineName)
        {
            return _lines[lineName];
        }

        public FindLineManager(Dictionary<string, FindLineFeeding> findLineFeedings,
            SnackbarMessageQueue messageQueue = null)
        {
            FindLineFeedings = findLineFeedings;
            CrossesIgnored.GenEmptyObj();
            LineRegions.GenEmptyObj();
            FindLineRects.GenEmptyObj();
            Edges.GenEmptyObj();
            MessageQueue = messageQueue;
        }

        public FindLineManager(SnackbarMessageQueue messageQueue = null)
        {
            CrossesIgnored.GenEmptyObj();
            LineRegions.GenEmptyObj();
            FindLineRects.GenEmptyObj();
            Edges.GenEmptyObj();
            MessageQueue = messageQueue;
        }


        public int CrossSize { get; set; } = 100;

        public double CrossAngle { get; set; } = 0.8;

        public HObject Edges
        {
            get { return _edges; }
            set { _edges = value; }
        }
    }
}