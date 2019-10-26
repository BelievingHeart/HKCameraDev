using System.Collections.Generic;

namespace ImageDebugger.Core.IoC.Interface
{
    /// <summary>
    /// Abstract the service of providing images
    /// </summary>
    public interface IImageProvider
    {
        List<string> GetImages();
    }
}