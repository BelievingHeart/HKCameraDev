using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HKCameraDev.Core.Enums;
using MvCamCtrl.NET;

namespace HKCameraDev.Core.ViewModels.CameraViewModel
{
    /// <summary>
    /// Manages HK cameras attached
    /// </summary>
    public static class HKCameraManager
    {
        /// <summary>
        /// Gets all the attached cameras on this machine
        /// Key = camera friendly name or camera serial name
        /// </summary>
        public static List<global::HKCameraDev.Core.ViewModels.CameraViewModel.CameraViewModel> AttachedCameras { get; private set; }

        /// <summary>
        /// Get a camera with a specific name
        /// </summary>
        /// <param name="name">camera name</param>
        /// <returns></returns>
        public static global::HKCameraDev.Core.ViewModels.CameraViewModel.CameraViewModel GetCameraByName(string name)
        {
            return AttachedCameras.First(cam => cam.Name == name);
        }

        /// <summary>
        /// Names for the attached cameras
        /// </summary>
        public static IEnumerable<string> CameraNames
        {
            get { return AttachedCameras?.Select(cam => cam.Name); }
        }

        /// <summary>
        /// Try to get information of all the attached cameras
        /// and store it as internal data
        /// This should be called when a camera is attached to or removed from a machine
        /// </summary>
        /// <returns>Indicates if init successes</returns>
        public static bool ScannedForAttachedCameras()
        {
            // Try to get all the information of the attached cameras
            MyCamera.MV_CC_DEVICE_INFO_LIST cameraList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            var nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE,
                ref cameraList);
            var success = nRet == 0;
            if (!success) return false;

            var cameraInfos = new List<MyCamera.MV_CC_DEVICE_INFO>();
            // Store camera information as internal data

            for (int i = 0; i < cameraList.nDeviceNum; i++)
            {
                var infoPtr = cameraList.pDeviceInfo[i];
                var info = (MyCamera.MV_CC_DEVICE_INFO) Marshal.PtrToStructure(infoPtr,
                    typeof(MyCamera.MV_CC_DEVICE_INFO));
                cameraInfos.Add(info);
            }

            GenerateCameraInstances(cameraInfos);

            HasInitiated = true;
            return true;
        }

        public static bool HasInitiated { get; set; }

        /// <summary>
        /// Generate wrappers for native HK cameras
        /// </summary>
        /// <param name="cameraInfos"></param>
        /// <exception cref="NotSupportedException">When the camera type is unsupported</exception>
        private static void GenerateCameraInstances(List<MyCamera.MV_CC_DEVICE_INFO> cameraInfos)
        {
            AttachedCameras = new List<global::HKCameraDev.Core.ViewModels.CameraViewModel.CameraViewModel>();
            // If there is any camera attached ...
            for (int i = 0; i < cameraInfos.Count; i++)
            {
                var cameraInfo = cameraInfos[i];
                // Determine camera type and camera name as well as ip address
                CameraType cameraType;
                string ip = string.Empty, cameraName;
                // If it is a gige camera...
                if (cameraInfo.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    cameraType = CameraType.Gige;
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(cameraInfo.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo =
                        (MyCamera.MV_GIGE_DEVICE_INFO) Marshal.PtrToStructure(buffer,
                            typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    // ch:显示IP | en:Display IP
                    UInt32 nIp1 = (gigeInfo.nCurrentIp & 0xFF000000) >> 24;
                    UInt32 nIp2 = (gigeInfo.nCurrentIp & 0x00FF0000) >> 16;
                    UInt32 nIp3 = (gigeInfo.nCurrentIp & 0x0000FF00) >> 8;
                    UInt32 nIp4 = (gigeInfo.nCurrentIp & 0x000000FF);
                    ip = nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4;

                    if (gigeInfo.chUserDefinedName != "")
                    {
                        cameraName = gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")";
                    }
                    else
                    {
                        cameraName = gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" +
                                     gigeInfo.chSerialNumber + ")";
                    }
                }
                // If it is a usb camera ...
                else if (cameraInfo.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    cameraType = CameraType.USB;
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(cameraInfo.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo =
                        (MyCamera.MV_USB3_DEVICE_INFO) Marshal.PtrToStructure(buffer,
                            typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        cameraName = usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")";
                    }
                    else
                    {
                        cameraName = usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" +
                                     usbInfo.chSerialNumber;
                    }
                }
                else
                {
                    throw new NotSupportedException("Can not support this type of camera currently");
                }

                AttachedCameras.Add(new global::HKCameraDev.Core.ViewModels.CameraViewModel.CameraViewModel()
                {
                    Name = cameraName,
                    CameraInfo = cameraInfo,
                    CameraType = cameraType,
                    IpAddress = ip
                });
            }
        }

        public static int NumCameras
        {
            get
            {
                if (!HasInitiated)
                    throw new InvalidOperationException("ScannedForAttachedCameras should be called first");
                return
                    AttachedCameras.Count;
            }
        }

/// <summary>
/// Closes all the attached cameras
/// </summary>
        public static void CloseAllCameras()
        {
            foreach (var camera in AttachedCameras)
            {
                camera.IsOpened = false;
            }
        }
    }
}