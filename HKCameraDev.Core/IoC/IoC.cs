using HKCameraDev.Core.IoC.Interface;
using HKCameraDev.Core.ViewModels.CameraViewModel;
using Ninject;

namespace HKCameraDev.Core.IoC
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();
        

        #endregion

        #region Construction

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all 
        ///       services can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();
            
            // Init cameras
            var success = HKCameraManager.ScannedForAttachedCameras();
            if(!success) Log("Failed to scan cameras");
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
        }

        #endregion

        /// <summary>
        /// Get a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        #region Property Shortcuts
        

        #endregion

        public static void Log(string message)
        {
            Get<IUILogger>().Log("Warning: " + message);
        }

    }
}