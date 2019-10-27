using System.Collections.Generic;
using HKCameraDev.Core.ViewModels.CameraViewModel;

namespace HKCameraDev.Core.ViewModels.CameraHostViewModel
{
    public class CameraHostViewModel : ViewModelBase
    {
        /// <summary>
        /// Names of all attached cameras
        /// </summary>
        public IEnumerable<string> CameraNames
        {
            get { return HKCameraManager.CameraNames; }
        }

        /// <summary>
        /// Name of the current camera to show
        /// </summary>
        public string CurrentCameraName { get; set; }
    }

}