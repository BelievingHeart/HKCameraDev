using System.Collections.Generic;
using System.Collections.ObjectModel;
using ImageDebugger.Core.Models;

namespace ImageDebugger.Core.ImageProcessing.TopView
{
    public sealed partial class I94TopViewMeasure
    {
        #region Hard Code Values

        public ObservableCollection<FaiItem> GenFaiItemValues(string faiItemSerializationDir)
        {
            var outputs = new ObservableCollection<FaiItem>();
            outputs.Add(new FaiItem("02_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 15.49, MinBoundary = 15.390});

            outputs.Add(new FaiItem("03_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 15.906, MinBoundary = 15.806});

            outputs.Add(new FaiItem("04_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 34.92, MinBoundary = 34.82});
            outputs.Add(new FaiItem("04_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 34.92, MinBoundary = 34.82});
            outputs.Add(new FaiItem("04_3")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 34.92, MinBoundary = 34.82});

            outputs.Add(new FaiItem("05_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 36.15, MinBoundary = 36.05});
            outputs.Add(new FaiItem("05_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 36.15, MinBoundary = 36.05});
            outputs.Add(new FaiItem("05_3")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 36.15, MinBoundary = 36.05});

            outputs.Add(new FaiItem("06_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 22.153, MinBoundary = 22.053});
            outputs.Add(new FaiItem("06_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 22.153, MinBoundary = 22.053});
            outputs.Add(new FaiItem("06_3")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 22.153, MinBoundary = 22.053});

            outputs.Add(new FaiItem("09_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 11.231, MinBoundary = 11.131});
            outputs.Add(new FaiItem("09_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 11.231, MinBoundary = 11.131});
            outputs.Add(new FaiItem("09_3")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 11.231, MinBoundary = 11.131});

            outputs.Add(new FaiItem("12_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 18.805, MinBoundary = 18.705});
            outputs.Add(new FaiItem("12_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 18.805, MinBoundary = 18.705});

            outputs.Add(new FaiItem("16_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 29.599, MinBoundary = 29.499});
            outputs.Add(new FaiItem("16_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 29.599, MinBoundary = 29.499});

            outputs.Add(new FaiItem("17_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 17.154, MinBoundary = 17.054});
            outputs.Add(new FaiItem("17_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 17.154, MinBoundary = 17.054});

            outputs.Add(new FaiItem("19_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 1.658, MinBoundary = 1.558});
            outputs.Add(new FaiItem("19_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 1.658, MinBoundary = 1.558});

            outputs.Add(new FaiItem("20_1")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 19.353, MinBoundary = 19.253});
            outputs.Add(new FaiItem("20_2")
                {SerializationDir = faiItemSerializationDir, MaxBoundary = 0.06, MinBoundary = 0});

            return outputs;
        }


        public List<FindLineLocation> GenFindLineLocationValues()
        {
            var frontLightIndex = 0;
            var backLightIndex = 1;
            return new List<FindLineLocation>()
            {
                new FindLineLocation()
                {
                    Name = "02-left", Angle = 90, X = 694, Y = 2017.3, Len2 = 218, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Positive
                },

                new FindLineLocation()
                {
                    Name = "02-right", Angle = 90, X = 1712, Y = 2017, Len2 = 218, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "03-left", Angle = -90, X = 1759, Y = 2077, Len2 = 274, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "03-right", Angle = -90, X = 679, Y = 2077, Len2 = 274, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "04", Angle = 90, X = 1224, Y = 4562, Len2 = 1024, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "05-1", Angle = 90, X = 2030, Y = 4724, Len2 = 250, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "05-2", Angle = 90, X = 1166, Y = 4724, Len2 = 210, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "05-3", Angle = 90, X = 366, Y = 4724, Len2 = 210, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                // upper left h line
                new FindLineLocation()
                {
                    Name = "09", Angle = -90, X = 3171, Y = 1466, Len2 = 317, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                // lower left h line
                new FindLineLocation()
                {
                    Name = "06", Angle = 90, X = 3162, Y = 2895.5, Len2 = 310, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "12-1", Angle = 0, X = 2451, Y = 3149, Len2 = 95, ImageIndex = backLightIndex, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "12-2", Angle = 0, X = 2451, Y = 3767, Len2 = 95, ImageIndex = backLightIndex, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "12-3", Angle = 0, X = 2451, Y = 4300, Len2 = 130, ImageIndex = backLightIndex, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "17-1", Angle = 0, X = 2241, Y = 326, Len2 = 95, ImageIndex = backLightIndex, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "17-2", Angle = 0, X = 2241, Y = 736, Len2 = 95, ImageIndex = backLightIndex, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    //95
                    Name = "17-3", Angle = 0, X = 2241, Y = 1149, Len2 = 90, ImageIndex = backLightIndex, Len1 = 20, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Angle = 0,
                    Name = "16", ImageIndex = backLightIndex, Len2 = 366, X = 3864.5, Y = 2147.5, Len1 = 25, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                {
                    Name = "19", Angle = 180, X = 206.5, Y = 705, Len2 = 375, ImageIndex = backLightIndex, Polarity = FindLinePolarity.Positive
                },
                new FindLineLocation()
                { //85
                    Name = "20.bottomLeft", Angle = -135, X = 2101, Y = 1945, Len2 = 60, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Negative
                },
                new FindLineLocation()
                { //73
                    Name = "20.topRight", Angle = 45, X = 347.5, Y = 133, Len2 = 50, ImageIndex = frontLightIndex, Polarity = FindLinePolarity.Negative
                }
            };
        }

        public ObservableCollection<FindLineParam> GenFindLineParamValues(string paramSerializationBaseDir)
        {
            var outputs = new ObservableCollection<FindLineParam>()
            {
                new FindLineParam()
                {
                    Name = "02-left",  Threshold = 2,
                    MinWidth = 1, MaxWidth = 10,  WhichEdge = EdgeSelection.Last
                },
                new FindLineParam()
                {
                    Name = "02-right",  Threshold = 3,
                    MinWidth = 1, MaxWidth = 10,  WhichEdge = EdgeSelection.Last
                },
                new FindLineParam()
                {
                    Name = "03-left",   Threshold = 10, IgnoreFraction = 0.2
                },
                new FindLineParam()
                {
                    Name = "03-right",   Threshold = 10, IgnoreFraction = 0.2
                },

                new FindLineParam()
                {
                    Name = "04",  Threshold = 20, 
                },
                new FindLineParam()
                {
                    Name = "05-1",  Threshold = 10, CannyHigh = 30, CannyLow = 15,
                    
                },
                new FindLineParam()
                {
                    Name = "05-2",  Threshold = 10, CannyHigh = 30, CannyLow = 15,
                    
                },
                new FindLineParam()
                {
                    Name = "05-3",  Threshold = 10, CannyHigh = 30, CannyLow = 15,
                    
                },
                new FindLineParam()
                {
                    Name = "06",  Threshold = 10, CannyHigh = 30, CannyLow = 15
                },
                new FindLineParam()
                {
                    Name = "09",  Threshold = 10, CannyHigh = 30, CannyLow = 15
                },
                new FindLineParam()
                {
                    Name = "12-1",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },
                new FindLineParam()
                {
                    Name = "12-2",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },
                new FindLineParam()
                {
                    Name = "12-3",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },
                new FindLineParam()
                {
                    Name = "17-1",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },
                new FindLineParam()
                {
                    Name = "17-2",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },
                new FindLineParam()
                {
                    Name = "17-3",  Threshold = 10, CannyHigh = 30, CannyLow = 15, IgnoreFraction = 0.1
                },


                new FindLineParam()
                {
                    Name = "16",   Threshold = 10, 
                },
                new FindLineParam()
                {
                    Name = "19",  Threshold = 10, CannyHigh = 30, CannyLow = 15,
                },
                new FindLineParam()
                {
                    Name = "20.bottomLeft",  Threshold = 30, NewWidth = 3, Sigma1 = 5, Sigma2 = 3, 
                },
                new FindLineParam()
                {
                    Name = "20.topRight",  Threshold = 20, NewWidth = 3
                },
                new FindLineParam()
                {
                    Name = "TopBase"
                },
                new FindLineParam()
                {
                    Name = "RightBase"
                },
            };

            foreach (var param in outputs)
            {
                param.SerializationDir = paramSerializationBaseDir;
            }

            return outputs;
        }

        #endregion
    }
}