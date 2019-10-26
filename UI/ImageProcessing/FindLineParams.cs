using System.Xml;
using System.Xml.Serialization;

namespace UI.ImageProcessing
{
    public class FindLineParams
    {
        [XmlElement] public string Name { get; set; } = "Null";
        [XmlElement] public FindLinePolarity Polarity { get; set; } = FindLinePolarity.Positive;
        [XmlElement] public EdgeSelection WhichEdge { get; set; } = EdgeSelection.First;
        [XmlElement] public PairSelection WhichPair { get; set; } = PairSelection.First;
        [XmlElement] public int Threshold { get; set; } = 20;
        [XmlElement] public double IgnoreFraction { get; set; } = 0.2;
        [XmlElement] public int NewWidth { get; set; } = 5;
        [XmlElement] public double Sigma1 { get; set; } = 1;
        [XmlElement] public double Sigma2 { get; set; } = 1;
        [XmlElement] public int CannyLow { get; set; } = 20;
        [XmlElement] public int CannyHigh { get; set; } = 40;
        [XmlElement] public bool FirstStepOnly { get; set; } = false;

        [XmlElement] public bool UsingPair { get; set; } = false;
        [XmlElement] public int MinWidth { get; set; }
        [XmlElement] public int MaxWidth { get; set; }
    }


    public enum FindLinePolarity
    {
        Positive,
        Negative
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