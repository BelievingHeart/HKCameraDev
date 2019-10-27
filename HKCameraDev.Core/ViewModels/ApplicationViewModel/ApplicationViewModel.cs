using System;
using MaterialDesignThemes.Wpf;

namespace HKCameraDev.Core.ViewModels.ApplicationViewModel
{
    public class ApplicationViewModel : ViewModelBase
    {
        private static ApplicationViewModel _Instance = new ApplicationViewModel()
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000))
        };

        /// <summary>
        /// Application wide instance for xaml to bind to
        /// </summary>
        public static ApplicationViewModel Instance
        {
            get { return _Instance; }
        }

        /// <summary>
        /// Message queue for ui logging
        /// </summary>
        public ISnackbarMessageQueue MessageQueue { get; set; }
    }

}