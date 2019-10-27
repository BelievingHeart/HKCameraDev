using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using HalconDotNet;

namespace HKCameraDev.Core.ViewModels.CameraViewModel
{
    public partial class CameraViewModel
    {
        private string _serializeDir;

        public ICommand ResetSaveImageCommand { get; set; }

        /// <summary>
        /// Batch size of images to be saved
        /// </summary>
        public int SaveImageBatch { get; set; } = 1;

        /// <summary>
        /// The path to save images
        /// </summary>
        public string SerializeDir
        {
            get { return _serializeDir; }
            set { _serializeDir = value.Contains("\"") ? value.Replace("\"", "") : value; }
        }

        /// <summary>
        /// Bitmaps to be saved
        /// </summary>
        public Queue<HImage> ImagesInMemory { get; set; } = new Queue<HImage>();

        /// <summary>
        /// Index of the current image 
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Whether images should be save
        /// </summary>
        public bool ShouldSaveImage { get; set; } = false;

        /// <summary>
        /// How many images is currently in memory
        /// </summary>
        public int NumImagesInMemory { get; set; }
        
        
        /// <summary>
        /// Add images to list to be saved
        /// </summary>
        /// <param name="image"></param>
        private void AddImageThreadSafe(HImage image)
        {

            lock (ImagesInMemory)
            {
                ImagesInMemory.Enqueue(image);
                NumImagesInMemory = ImagesInMemory.Count;
                CurrentIndex++;
                
                if (SavePointReached) SerializeImages();
            }
            
        }

        private void SerializeImages()
        {
            if (!string.IsNullOrEmpty(SerializeDir) && ShouldSaveImage)
            {
                Directory.CreateDirectory(SerializeDir);
                for (int i = 0; i < SaveImageBatch; i++)
                {
                    var saveIndex = CurrentIndex - SaveImageBatch + i;
                    var imagePath = SerializeDir + "/" + saveIndex + ".bmp";
                    var image = ImagesInMemory.Dequeue();
                    HOperatorSet.WriteImage(image, "bmp", 0, imagePath);
                }
            }
        }

        private void ResetSaveImages()
        {
            ImagesInMemory.Clear();
            CurrentIndex = 0;
            NumImagesInMemory = 0;
        }

        

        public bool SavePointReached => CurrentIndex % SaveImageBatch == 0;
    }
}