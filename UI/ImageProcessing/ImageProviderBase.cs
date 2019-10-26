using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.ImageProcessing
{
    public class ImageProviderBase
    {
        protected bool IsImageFile(string imagePath)
        {
            return new List<string>()  { ".JPG", ".JPE", ".BMP", ".TIF", ".PNG" } .Contains(Path.GetExtension(imagePath)?.ToUpper());
        }

        protected string GetImageName(string imagePath)
        {
            return Path.GetFileName(imagePath);
        }
    }
}
