using System.Collections.Generic;
using ImageDebugger.Core.ImageProcessing;

namespace ImageDebugger.Core.Models
{
    public class ImageProcessingResult
    {
        public Dictionary<string, double> FaiDictionary { get; set; }

        public HalconGraphics HalconGraphics { get; set; }

        public DataRecorder DataRecorder { get; set; }
    }
}