using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UI.Helpers;

namespace ImageDebugger.Core.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Preserved for manual invocation of property changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Run async action that is only allowed to run one instance each time
        /// This workaround exists because property can not be passed by ref
        /// For example, to avoid multiple button clicks before a task is finished
        /// The action is allowed to run if the wrapped property is false, i.e. not busy
        /// If the action is finished, the action is allowed to run again
        /// </summary>
        /// <param name="isBusyExpression">The expression that wraps a boolean property</param>
        /// <param name="action">The task to run</param>
        /// <returns></returns>
        protected async Task RunOnlySingleFireIsAllowedEachTimeCommand(Expression<Func<bool>> isBusyExpression,
            Func<Task> action)
        {
            // Check if the system is busy
            if (isBusyExpression.GetPropertyValue()) return;
            // Flag the system busy before task is run
            isBusyExpression.SetPropertyValue(true);
            // Execute task if the system is not busy
            try
            {
                await action();
            }
            finally
            {
                // Flag the system not-busy again after task is finished
                isBusyExpression.SetPropertyValue(false);
            }
        }
    }}