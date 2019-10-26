using System.Collections.Generic;
using System.Collections.ObjectModel;
using UI.Models;

namespace UI.ImageProcessing.BottomView
{
    public partial class I94BottomViewMeasure
    {
        public int NumImagesInOneGoRequired { get; } = 1;

        public ObservableCollection<FaiItem> GenFaiItemValues(string faiItemSerializationDir)
        {
            var outputs = new ObservableCollection<FaiItem>()
            {
                new FaiItem("21_1")
                {
                    MaxBoundary = 17.52, MinBoundary = 17.42, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("21_2")
                {
                    MaxBoundary = 17.52, MinBoundary = 17.42, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("23_1")
                {
                    MaxBoundary = 18.59, MinBoundary = 18.49, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("23_2")
                {
                    MaxBoundary = 18.59, MinBoundary = 18.49, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("24_1")
                {
                    MaxBoundary = 12.16, MinBoundary = 12.06, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("25_1")
                {
                    MaxBoundary = 10.823, MinBoundary = 10.723, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("25_2")
                {
                    MaxBoundary = 0.06, MinBoundary = 0, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("26_1")
                {
                    MaxBoundary = 3.41, MinBoundary = 3.31, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("26_2")
                {
                    MaxBoundary = 3.41, MinBoundary = 3.31, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("27_1")
                {
                    MaxBoundary = 4.283, MinBoundary = 4.183, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("27_2")
                {
                    MaxBoundary = 4.283, MinBoundary = 4.183, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("28_1")
                {
                    MaxBoundary = 8.953, MinBoundary = 8.853, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("28_2")
                {
                    MaxBoundary = 0.06, MinBoundary = 0, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("29_1")
                {
                    MaxBoundary = 4.283, MinBoundary = 4.183, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("29_2")
                {
                    MaxBoundary = 4.283, MinBoundary = 4.183, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("31_1")
                {
                    MaxBoundary = 0.06, MinBoundary = 0, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("32_1")
                {
                    MaxBoundary = 0.06, MinBoundary = 0, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("33_1")
                {
                    MaxBoundary = 0.06, MinBoundary = 0, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("123_1")
                {
                    MaxBoundary = 35.27, MinBoundary = 35.17, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("123_2")
                {
                    MaxBoundary = 35.27, MinBoundary = 35.17, SerializationDir = faiItemSerializationDir
                },
                new FaiItem("123_3")
                {
                    MaxBoundary = 35.27, MinBoundary = 35.17, SerializationDir = faiItemSerializationDir
                }
            };

            return outputs;
        }

        public List<FindLineLocation> GenFindLineLocationValues()
        {
            var outputs = new List<FindLineLocation>()
            {
                new FindLineLocation()
                {
                    //p
                    Name = "21left-top", X = 72, Y = 2679, Angle = 0, Len2 = 148, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "21left-bottom", X = 80, Y = 3866, Angle = 0, Len2 = 130, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "21right-top", X = 2322, Y = 2671, Angle = 180, Len2 = 145, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "21right-bottom", X = 2323, Y = 3817, Angle = 180, Len2 = 145, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "23top", X = 1208, Y = 2069, Angle = 90, Len2 = 610, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    // n
                    Name = "23bottom-left", X = 660, Y = 4450, Angle = 90, Len2 = 114, ImageIndex = 0,
                    Polarity = FindLinePolarity.Negative
                },
                new FindLineLocation()
                {
                    // n
                    Name = "23bottom-center", X = 1189, Y = 4448, Angle = 90, Len2 = 215, ImageIndex = 0,
                    Polarity = FindLinePolarity.Negative
                },
                new FindLineLocation()
                {
                    // n
                    Name = "23bottom-right", X = 1750, Y = 4448, Angle = 90, Len2 = 114, ImageIndex = 0,
                    Polarity = FindLinePolarity.Negative
                },
                new FindLineLocation()
                {
                    // p
                    Name = "24.left", X = 414, Y = 1016, Angle = 0, Len2 = 145, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    // p
                    Name = "24.right", X = 1973, Y = 1016, Angle = 180, Len2 = 145, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    // p
                    Name = "27.top", X = 3140, Y = 1590, Angle = 90, Len2 = 150, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "27.bottom", X = 3140, Y = 2679, Angle = -90, Len2 = 150, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    // p
                    Name = "29.left", X = 2602, Y = 2134, Angle = 0, Len2 = 150, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "29.right", X = 3684, Y = 2134, Angle = 180, Len2 = 150, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    // p
                    Name = "31.topLeft", X = 283, Y = 2278, Angle = 45, Len2 = 240, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    //p
                    Name = "31.bottomLeft", X = 282, Y = 4238, Angle = -45, Len2 = 240, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    Name = "31.topRight", X = 2109, Y = 2275, Angle = 135, Len2 = 240, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    Name = "31.bottomRight", X = 2112, Y = 4236, Angle = -135, Len2 = 240, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    Name = "123-left", X = 393, Y = 4533, Angle = -90, Len2 = 230, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    Name = "123-center", X = 1169, Y = 4531, Angle = -90, Len2 = 170, ImageIndex = 0
                },
                new FindLineLocation()
                {
                    Name = "123-right", X = 2000, Y = 4527, Angle = -90, Len2 = 230, ImageIndex = 0
                },
            };
            return outputs;
        }

        public ObservableCollection<FindLineParam> GenFindLineParamValues(string paramSerializationBaseDir)
        {
            var outputs = new ObservableCollection<FindLineParam>()
            {
                new FindLineParam()
                {
                    Name = "21left-top"
                },
                new FindLineParam()
                {
                    Name = "21left-bottom"
                },
                new FindLineParam()
                {
                    Name = "21right-top"
                },
                new FindLineParam()
                {
                    Name = "21right-bottom"
                },
                new FindLineParam()
                {
                    Name = "23top"
                },
                new FindLineParam()
                {
                    Name = "23bottom-left"
                },
                new FindLineParam()
                {
                    Name = "23bottom-center"
                },
                new FindLineParam()
                {
                    Name = "23bottom-right"
                },
                new FindLineParam()
                {
                    Name = "24.left"
                },
                new FindLineParam()
                {
                    Name = "24.right"
                },
                new FindLineParam()
                {
                    Name = "27.top"
                },
                new FindLineParam()
                {
                    Name = "27.bottom"
                },
                new FindLineParam()
                {
                    Name = "29.left"
                },
                new FindLineParam()
                {
                    Name = "29.right"
                },
                new FindLineParam()
                {
                    Name = "31.topLeft"
                },
                new FindLineParam()
                {
                    Name = "31.bottomLeft"
                },
                new FindLineParam()
                {
                    Name = "31.topRight"
                },
                new FindLineParam()
                {
                    Name = "31.bottomRight"
                },
                new FindLineParam()
                {
                    Name = "123-left", MinWidth = 1, MaxWidth = 15, WhichEdge = EdgeSelection.Last, Threshold = 5
                },
                new FindLineParam()
                {
                    Name = "123-center", MinWidth = 1, MaxWidth = 15, WhichEdge = EdgeSelection.Last, Threshold = 5
                },
                new FindLineParam()
                {
                    Name = "123-right", MinWidth = 1, MaxWidth = 15, WhichEdge = EdgeSelection.Last, Threshold = 5
                },
                new FindLineParam()
                {
                    Name = "TopBase"
                },
                new FindLineParam()
                {
                    Name = "LeftBase"
                },
            };

            foreach (var item in outputs)
            {
                item.SerializationDir = paramSerializationBaseDir;
            }

            return outputs;
        }

        public Dictionary<string, FindLineLocation> EdgeLocationsRelative { get; set; } = new Dictionary<string, FindLineLocation>()
        {
            {
                "26-leftTop", new FindLineLocation()
                {
                    X = 478, Y = 795, Angle = 90, Len1 = 130, Len2 = 92
                }
            },
            {
                "26-leftBottom", new FindLineLocation()
                {
                    X = 478, Y = 1161, Angle = 90, Len1 = 130, Len2 = 92
                }
            }, 
            {
                "26-rightTop", new FindLineLocation()
                {
                    X = 1905, Y = 795, Angle = 90, Len1 = 130, Len2 = 92
                }
            },
            {
                "26-rightBottom", new FindLineLocation()
                {
                    X = 1905, Y = 1161, Angle = 90, Len1 = 130, Len2 = 92
                }
            },
            {
                "leftCircle", new FindLineLocation()
                {
                    X = 1210, Y = 1022, Angle = 90, Len1 = 847, Len2 = 617
                }
            },
            {
                "rightCircle", new FindLineLocation()
                {
                    X = 3140, Y = 2116, Angle = 90, Len1 = 635, Len2 = 617
                }
            },

        };
    }
}