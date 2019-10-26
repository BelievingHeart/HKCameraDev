using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.ImageProcessing
{
    public partial class I94TopViewMeasure
    {
        public void ReadImageDirectory(string imageDirectory)
        {
            
        }

        public List<string> NextImage { get; }
        public List<string> LastImage { get; }
        public ObservableCollection<string> ImageNames { get; }
        public List<string> GetImageByName(string imageName)
        {
            throw new NotImplementedException();
        }
    }
}
