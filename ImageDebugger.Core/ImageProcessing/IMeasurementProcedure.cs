using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HalconDotNet;
using ImageDebugger.Core.Models;
using MaterialDesignThemes.Wpf;

namespace ImageDebugger.Core.ImageProcessing
{
    public interface IMeasurementProcedure
    {

        /// <summary>
        /// This is convenient for assigning config and output directory 
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Specifies haw many images is needed in single run of image processing
        /// </summary>
        int NumImagesInOneGoRequired { get; }

 
/// <summary>
/// Where the image processing happens
/// </summary>
/// <param name="images">Input images</param>
/// <param name="findLineConfigs">Params for finding each line</param>
/// <param name="faiItems">List to update the measured values</param>
/// <param name="indexToShow">Which image of the input images should be shown</param>
/// <param name="messageQueue">A message queue for outputting debugging informations</param>
/// <returns></returns>
        Task<ImageProcessingResult> ProcessAsync(List<HImage> images, FindLineConfigs findLineConfigs,
            ObservableCollection<FaiItem> faiItems, int indexToShow, SnackbarMessageQueue messageQueue
          );

/// <summary>
/// Generate settings for all fai items to measure usually at the first time to run
/// </summary>
/// <param name="faiItemSerializationDir">The serialization directory for the fai items</param>
/// <returns></returns>
         ObservableCollection<FaiItem> GenFaiItemValues(string faiItemSerializationDir);

/// <summary>
/// Generate locations for all the lines to be found,
/// this will be call each time the application is run
/// because there is no reason to change the locations to find line
/// we don't expose them to serialization or UI editing
/// </summary>
/// <returns></returns>
         List<FindLineLocation> GenFindLineLocationValues();

/// <summary>
/// Generate find-line settings for all lines to be found usually at the first time to run
/// </summary>
/// <param name="paramSerializationBaseDir">The serialization directory of the find-line params</param>
/// <returns></returns>
         ObservableCollection<FindLineParam> GenFindLineParamValues(string paramSerializationBaseDir);
        
    }
}