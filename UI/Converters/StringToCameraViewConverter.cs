﻿﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
  using HKCameraDev.Core.ViewModels.ApplicationViewModel;
  using HKCameraDev.Core.ViewModels.CameraViewModel;
  using UI.Views.CameraView;


  namespace UI.Converters
{
    public class StringToCameraViewConverter : ValueConverterBase<StringToCameraViewConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var cameraName = (string) value;
            return RetrievePage(cameraName);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the requested camera view model
        /// </summary>
        /// <param name="cameraName">name of the camera</param>
        /// <returns></returns>
        private static UserControl RetrievePage(string cameraName)
        {
            var dataContext = HKCameraManager.GetCameraByName(cameraName);
            
            return new CameraView()
            {
                DataContext = dataContext
            };
        }
        

    }
}