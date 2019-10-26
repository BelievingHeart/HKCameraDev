using HalconDotNet;

namespace UI.Interfaces
{
    public interface IImageProvider
    {
        HImage NextImage { get; }
        int NumImages { get; }
    }
}