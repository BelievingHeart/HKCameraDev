using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ImageDebugger.Core.IoC.Interface;

namespace UI.DataAccess
{
    public class ImageProvider : IImageProvider
    {
        /// <summary>
        /// Known list of image extensions to filter non-image files
        /// </summary>
        private static readonly List<string> ImageExtensions = new List<string>
            {".JPG", ".JPE", ".BMP", ".TIF", ".PNG"};


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
        /// Provide paths to images
        /// </summary>
        /// <returns></returns>
        public List<string> GetImages()
        {
            var outputs = new List<string>();

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                var selectedPath = fbd.SelectedPath;
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(selectedPath))
                {
                    string[] filePaths = Directory.GetFiles(selectedPath);

                    foreach (var imagePath in filePaths)
                    {
                        if (IsImageFile(imagePath))
                        {
                            outputs.Add(imagePath);
                        }
                    }
                }
            }

            return outputs;
        }
    }
}