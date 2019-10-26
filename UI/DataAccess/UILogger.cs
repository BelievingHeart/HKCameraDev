using ImageDebugger.Core.IoC.Interface;
using ImageDebugger.Core.ViewModels.ApplicationViewModel;

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