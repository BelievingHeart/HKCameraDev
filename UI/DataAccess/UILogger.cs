using HKCameraDev.Core.IoC.Interface;
using HKCameraDev.Core.ViewModels.ApplicationViewModel;

namespace UI.DataAccess
{
    public class UILogger : IUILogger
    {
        public void Log(string message)
        {
            ApplicationViewModel.Instance.MessageQueue?.Enqueue(message);
        }
    }
}