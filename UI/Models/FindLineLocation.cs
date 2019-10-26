using System;
using System.Xml.Serialization;

namespace UI.Models
{
    public class FindLineLocation
    {
        [XmlAttribute] public FindLinePolarity Polarity { get; set; } = FindLinePolarity.Positive;
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public double X { get; set; }
        [XmlAttribute] public double Y { get; set; }
        [XmlAttribute] public double Angle { get; set; }
        [XmlAttribute] public double Len1 { get; set; } = 15;

        [XmlAttribute] public double Len2 { get; set; }

        /// <summary>
        /// Which image to find line
        /// </summary>
        [XmlAttribute] public int ImageIndex { get; set; }

        public string IsVertical
        {
            get { return (180 - Math.Abs(Angle) < 10 || Math.Abs(Angle) < 10) ? "true" : "false"; }
        }




    }
    public enum FindLinePolarity
    {
        Positive,
        Negative
    }
}