using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using UI.Commands;
using UI.Enums;

namespace UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Command to switch to bottom view page
        /// </summary>
        public ICommand SwitchBottomViewCommand { get; set; }

        /// <summary>
        /// Command to switch to top view page
        /// </summary>
        public ICommand SwitchTopViewCommand { get; set; }

        /// <summary>
        /// Current measurement page to show
        /// </summary>
        public MeasurementPage CurrentMeasurementPage { get; set; } = MeasurementPage.I94Top;

        public string CurrentMeasurementName
        {
            get { return CurrentMeasurementPage.ToString(); }
        }

        public MainWindowViewModel()
        {
            SwitchTopViewCommand = new RelayCommand(() =>
            {
                CurrentMeasurementPage = MeasurementPage.I94Top;
                MessageQueue.Enqueue("Switched to top view");
            });
            SwitchBottomViewCommand = new RelayCommand(() =>
            {
                CurrentMeasurementPage = MeasurementPage.I94Bottom;
                MessageQueue.Enqueue("Switched to bottom view");
            });
        }


        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3000));
    }
}