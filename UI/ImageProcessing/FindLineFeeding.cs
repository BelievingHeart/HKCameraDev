using HalconDotNet;

namespace UI.ImageProcessing
{
    /// <summary>
    /// A pack of parameter to feed in halcon's find line API
    /// </summary>
    public class FindLineFeeding
    {
        public HTuple Row { get; set; }

        public HTuple Col { get; set; }

        public HTuple Radian { get; set; }
        public HTuple Len1 { get; set; }
        public HTuple Len2 { get; set; }
        public HTuple Transition { get; set; }

        public HTuple NumSubRects { get; set; }
        public HTuple IgnoreFraction { get; set; }
        public HTuple Threshold { get; set; }
        public HTuple Sigma1 { get; set; }
        public HTuple Sigma2 { get; set; }
        public HTuple WhichEdge { get; set; }
        public HTuple WhichPair { get; set; }
        public HTuple NewWidth { get; set; }
        public HTuple MinWidth { get; set; }
        public HTuple MaxWidth { get; set; }

        public HTuple CannyHigh { get; set; }
        public HTuple CannyLow { get; set; }

        public bool FirstAttemptOnly { get; set; }

        public bool UsingPair { get; set; }

        public int ImageIndex { get; set; }

        public int MaxTrials { get; set; }

        public double Probability { get; set; }

        public double ErrorThreshold { get; set; }


        public HTuple IsVertical { get; set; }

        public int KernelWidth { get; set; }

        public HTuple LongestOnly { get; set; } = "false";
    }
}