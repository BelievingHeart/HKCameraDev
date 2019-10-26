using System.Windows;
using HKCameraDev.Core.IoC;
using HKCameraDev.Core.IoC.Interface;
using UI.DataAccess;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Set up IoC
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up IoC
            IoC.Kernel.Bind<IUILogger>().ToConstant(new UILogger());

            IoC.Setup();
            
            // Open main window
            var window = new MainWindow();
            Current.MainWindow = window;
            window.Show();
        }
    }
}