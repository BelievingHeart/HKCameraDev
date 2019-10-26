namespace ImageDebugger.Core.ViewModels
{
    public class ImageInfoViewModel : ViewModelBase
    {
        /// <summary>
        /// X coordinate of the cursor in image
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y coordinate of the cursor in image
        /// </summary>
        public int Y { get; set; }


        /// <summary>
        /// Gray value of the image point
        /// </summary>
        public int GrayValue { get; set; }
        
    }
}