using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace UI.ViewModels
{
    public partial class HalconWindowPageViewModel
    {
        #region Image Providing Logic

        /// <summary>
        /// Current processed image name to show
        /// </summary>
        public string CurrentImageName { get; private set; }

        /// <summary>
        /// The command to select a specific image and run image processing
        /// </summary>
        public ICommand ImageNameSelectionChangedCommand { get; }

        /// <summary>
        /// Log information to the UI
        /// </summary>
        /// <param name="message"></param>
        private void PromptUserThreadSafe(string message)
        {
            RunStatusMessageQueue.Enqueue(message);
        }
        
        /// <summary>
        /// Get the image name without directory
        /// </summary>
        /// <param name="imagePath">Full path to the image</param>
        /// <returns></returns>
        private string GetImageName(string imagePath)
        {
            return Path.GetFileName(imagePath);
        }


        /// <summary>
        /// Known list of image extensions to filter non-image files
        /// </summary>
        private static readonly List<string> ImageExtensions = new List<string>
            {".JPG", ".JPE", ".BMP", ".TIF", ".PNG"};

        private string _imageDirectory;

        /// <summary>
        /// Directory to images
        /// 1. If folder not contains any images do not update any state
        /// 2. If image names incorrect or image lists not equal, do not update any state
        /// 3. If naming correct and list all have the same count, update image lists, current image index and total images
        /// </summary>
        private string ImageDirectory
        {
            get { return _imageDirectory; }
            set
            {
                _imageDirectory = value;

                string[] filePaths = Directory.GetFiles(_imageDirectory);
                var imagePaths = new List<string>();

                foreach (var imagePath in filePaths)
                {
                    if (IsImageFile(imagePath))
                    {
                        imagePaths.Add(imagePath);
                    }
                }

                if (imagePaths.Count == 0)
                {
                    PromptUserThreadSafe("This folder does not contains any supported images");
                    return;
                }

                bool updateImageListsSuccess = TryAssignImageLists(imagePaths);

                // set it to empty so that user can reopen the same directory
                _imageDirectory = string.Empty;


                if (!updateImageListsSuccess) return;
                // Generate image names
                ImageNames = GenImageNames();
                ImageToShowSelectionList = GenImageToShowSelectionList(NumLists);
            }
        }

    /// <summary>
    /// Generate a list of image index to show
    /// The size of the list equals to the size of input images
    /// required for each image processing run
    /// </summary>
    /// <param name="numImagesOneGo"></param>
    /// <returns></returns>
        private List<int> GenImageToShowSelectionList(int numImagesOneGo)
        {
            var output = new List<int>();
            for (int i = 0; i < numImagesOneGo; i++)
            {
                output.Add(i);
            }

            return output;
        }
        
        /// <summary>
        /// Generate the image names to append in the combo box
        /// which can be used to selected an image and run image processing
        /// </summary>
        /// <returns>The generated image names</returns>
        private List<string> GenImageNames()
        {
            var output = new List<string>();
            foreach (var paths in this)
            {
                output.Add(GetImageName(paths[0]));
            }

            return output;
        }

        /// <summary>
        /// The separator to split image name from major index and minor index
        /// One major index corresponds to one image processing run
        /// One minor index corresponds to the order feed into the image processing within each run
        /// </summary>
        private string Separator { get; set; } = "-";

        /// <summary>
        /// Assign and return true only if
        /// all named correctly and all lists have the same count
        /// </summary>
        /// <param name="imagePaths">Paths to the images</param>
        /// <returns>Whether the assignment to the mega image list is successful</returns>
        private bool TryAssignImageLists(List<string> imagePaths)
        {
            int numImagesInOneGo = GetNumImagesInOneGo(imagePaths);

            if (numImagesInOneGo != MeasurementUnit.NumImagesInOneGoRequired)
            {
                PromptUserThreadSafe("Incorrect number of input images, check the image directory!");
                return false;
            }
            
            List<List<string>> tempMegaList = MakeTempMegaList(numImagesInOneGo);


            foreach (var path in imagePaths)
            {
                int imageIndex = 0;

                if (numImagesInOneGo > 1)
                {
                    var imageName = Path.GetFileName(path);
                    var start = imageName.IndexOf(Separator, StringComparison.Ordinal) + 1;
                    var length = 1;
                    var imageIndexString = imageName.Substring(start, length);
                    try
                    {
                        imageIndex = int.Parse(imageIndexString) - 1;
                    }
                    catch (Exception e)
                    {
                        PromptUserThreadSafe($"Incorrect image name: {imageName}");
                        return false;
                    }
                }

                tempMegaList[imageIndex].Add(path);
            }

            var numImagesInFirstList = tempMegaList[0].Count;
            if (tempMegaList.Any(l => l.Count != numImagesInFirstList))
            {
                PromptUserThreadSafe("Count of image lists not equal");
                return false;
            }

            var sortedImageMegaList = new List<List<string>>();
            foreach (var queue in tempMegaList)
            {
                var orderedQueue = new List<string>(queue.OrderBy(Path.GetFileName));
                sortedImageMegaList.Add(orderedQueue);
            }

            Reconstruct(sortedImageMegaList);
            return true;
        }

        /// <summary>
        /// Make a temporary list for storing image paths before assigning to
        /// the internal image mega list
        /// </summary>
        /// <param name="numImagesInOneGo">How many images are required for each image processing</param>
        /// <returns></returns>
        private List<List<string>> MakeTempMegaList(int numImagesInOneGo)
        {
            var output = new List<List<string>>();
            for (int i = 0; i < numImagesInOneGo; i++)
            {
                output.Add(new List<string>());
            }

            return output;
        }


        /// <summary>
        /// Determine how many images should be provided within one button hit
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <returns></returns>
        private int GetNumImagesInOneGo(List<string> imagePaths)
        {
            var allImageNames = imagePaths.Select(Path.GetFileName);
            var nameToTest = Path.GetFileName(imagePaths[0]);

            // Naming convention: images belong to the same group will have the same prefix
            // for example: 02_1 and 02_2 have the same prefix 02_
            if (!nameToTest.Contains(Separator)) return 1;

            var testPrefix = nameToTest.Substring(0, nameToTest.IndexOf(Separator, StringComparison.Ordinal) + 1);

            return allImageNames.Count(ele => ele.StartsWith(testPrefix));
        }

        /// <summary>
        /// Filter image files based on file extension
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private bool IsImageFile(string imagePath)
        {
            return ImageExtensions.Contains(Path.GetExtension(imagePath)?.ToUpper());
        }

        /// <summary>
        /// Command to open a image directory and assign mega image list
        /// </summary>
        public ICommand SelectImageDirCommand { get; private set; }
        
        /// <summary>
        /// Specifies whether there are images available for image processing 
        /// </summary>
        public bool HasImages => Count > 0;

        #endregion
    }
}