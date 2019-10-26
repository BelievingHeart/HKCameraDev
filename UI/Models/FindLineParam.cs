using System.IO;
using System.Xml.Serialization;
using PropertyChanged;
using UI.ImageProcessing;

namespace UI.Models
{
    public class FindLineParam : AutoSerializableBase<FindLineParam>
    {
        [DoNotNotify] [XmlAttribute] public string Name { get; set; }

        #region FirstAttempt

        [XmlAttribute] public EdgeSelection WhichEdge { get; set; } = EdgeSelection.First;
        [XmlAttribute] public int Threshold { get; set; } = 20;

        /// <summary>
        /// Number of measure rectangle to generate
        /// </summary>
        [XmlAttribute]
        public int NumSubRects { get; set; } = 10;

        [XmlAttribute] public double IgnoreFraction { get; set; } = 0.2;
        [XmlAttribute] public double Sigma1 { get; set; } = 1;

        #endregion

        #region FindEdges

        [XmlAttribute] public int NewWidth { get; set; } = 5;

        [XmlAttribute] public double Sigma2 { get; set; } = 1;
        [XmlAttribute] public int CannyLow { get; set; } = 20;
        [XmlAttribute] public int CannyHigh { get; set; } = 40;
        [XmlAttribute] public int KernelWidth { get; set; } = -1;
        [XmlAttribute] public bool LongestOnly { get; set; } = false;

        #endregion


        #region Fit Line

        [XmlAttribute] public double ErrorThreshold { get; set; } = 1.0;

        [XmlAttribute] public double Probability { get; set; } = 0.95;

        [XmlAttribute] public int MaxTrials { get; set; } = 100;

        #endregion

        #region Using Pair

        [XmlAttribute] public PairSelection WhichPair { get; set; } = PairSelection.First;

        [XmlAttribute] public int MinWidth { get; set; } = -1;
        [XmlAttribute] public int MaxWidth { get; set; } = -1;

        #endregion

        protected override string GetSerializationPath()
        {
            return Path.Combine(SerializationDir, Name + ".xml");
        }

        [XmlIgnore] public string SerializationDir;

        public bool FirstAttemptOnly()
        {
            return NewWidth <= 0;
        }


        public bool UsingPair()
        {
            return MinWidth > 0 && MaxWidth > MinWidth;
        }

        public FindLineFeeding ToFindLineFeeding()
        {
            return new FindLineFeeding()
            {
                WhichEdge = WhichEdge == EdgeSelection.First ? "first" : "last",
                WhichPair = WhichPair == PairSelection.First ? "first" : "last",
                Threshold = Threshold,
                IgnoreFraction = IgnoreFraction,
                NewWidth = NewWidth,
                Sigma1 = Sigma1,
                Sigma2 = Sigma2,
                CannyLow = CannyLow,
                CannyHigh = CannyHigh,
                FirstAttemptOnly = FirstAttemptOnly(),
                UsingPair = UsingPair(),
                MinWidth = MinWidth,
                MaxWidth = MaxWidth,
                NumSubRects = NumSubRects,
                ErrorThreshold = ErrorThreshold,
                Probability = Probability,
                MaxTrials = MaxTrials
            };
        }
    }


    public enum EdgeSelection
    {
        First,
        Last
    }

    public enum PairSelection
    {
        First,
        Last
    }
}