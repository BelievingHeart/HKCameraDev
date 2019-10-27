using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Input;
using HalconDotNet;
using HKCameraDev.Core.Commands;
using HKCameraDev.Core.Enums;
using MaterialDesignThemes.Wpf;
using MvCamCtrl.NET;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace HKCameraDev.Core.ViewModels.CameraViewModel
{
    public partial class CameraViewModel : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// Image buffer reserved
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Buffer size reserved for image
        /// </summary>
        UInt32 _bufferSize = 3072 * 2048 * 3;

        private byte[] _bufForSaveImage;


        private uint _buffSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        private MyCamera.MV_CC_DEVICE_INFO _cameraInfo;
        private bool _continuousMode;

        /// <summary>
        /// Must be declare as member,
        /// otherwise it will be garbage collected when <see cref="Open"/> is done
        /// </summary>
        private  MyCamera.cbOutputExdelegate _imageCallbackDelegate;

        private bool _isGrabbing;

        private bool _isOpened;


        private MyCamera _myCamera = new MyCamera();
        private bool _realTimeMode;
        private CameraTriggerSourceType _triggerSourceType;

        #endregion

        #region Constructor

        public CameraViewModel()
        {
            SoftwareExecuteOnceCommand = new SimpleCommand(o=>SoftwareExecuteOnce(), o=>!RealTimeMode && IsGrabbing && TriggerSourceType == CameraTriggerSourceType.Software);

            ToggleTriggerSourceCommand = new SimpleCommand(o=>  ToggleTriggerSource(o.ToString()), o=>IsGrabbing && !RealTimeMode);

            ResetSaveImageCommand = new RelayCommand(ResetSaveImages);
            
            ImageAcquired += bitMap =>
            {
                HObject hObject = Bitmap2HObjectBpp24(bitMap);
                var hImage = HobjectToHimage(hObject);
                AddImageThreadSafe(hImage);
                ImageDisplay = hImage;
            };

            // Set initial state so that toggles can work
            _realTimeMode = true;
            _isOpened = false;
            _isGrabbing = false;
        }

        #endregion





        #region Events

        /// <summary>
        /// Callback to fire when new image is acquired
        /// </summary>
        public event Action<Bitmap> ImageAcquired;

        #endregion

        private void ImageCallback(IntPtr pdata, ref MyCamera.MV_FRAME_OUT_INFO_EX pframeinfo, IntPtr puser)
        {
            var bitMap = ParseRawImageData(pdata, pframeinfo);
            OnImageAcquired(bitMap);
        }

        private HImage HobjectToHimage(HObject hobject)
        {
            HTuple pointer, type, width, height;
            HImage output = new HImage();
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            output.GenImage1(type, width, height, pointer);

            return output;
        }
        
        /// <summary>
        /// Logging mechanism
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            IoC.IoC.Log("Warning: " + message);
        }


        /// <summary>
        /// <see cref="TriggerSourceType"/> need to be re-assigned when
        /// real-time mode is off
        /// </summary>
        private void ReactivateTriggerSource()
        {
            var previousTriggerSource = TriggerSourceType;
            _triggerSourceType = CameraTriggerSourceType.None;
            TriggerSourceType = previousTriggerSource;
        }


        /// <summary>
        /// Can also convert 8-bit to 8-bit
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private HObject Bitmap2HObjectBpp24(Bitmap bmp)
        {
            HObject output;
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out output, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte",
                    0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Unable to convert hobject from bitmap");
            }

            return output;
        }

        protected virtual void OnImageAcquired(Bitmap obj)
        {
            ImageAcquired?.Invoke(obj);
        }

        private Bitmap ParseRawImageData(IntPtr pData, MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo)
        {
            int nRet;
            UInt32 newBufferSize = 0;
            Bitmap output;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();

            nRet = _myCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                throw new InvalidOperationException("Can not get payload size");
            }

            newBufferSize = stParam.nCurValue;

            if (newBufferSize > _bufferSize)
            {
                _bufferSize = newBufferSize;
                _buffer = new byte[_bufferSize];

                // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                _buffSizeForSaveImage = _bufferSize * 3 + 2048;
                _bufForSaveImage = new byte[_buffSizeForSaveImage];
            }

            MyCamera.MvGvspPixelType enDstPixelType;
            if (IsMonoData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            }
            else if (IsColorData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            }
            else
            {
                throw new NotSupportedException("Can not support such pixel type currently");
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(_bufForSaveImage, 0);

            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM
            {
                nWidth = stFrameInfo.nWidth,
                nHeight = stFrameInfo.nHeight,
                pSrcData = pData,
                nSrcDataLen = stFrameInfo.nFrameLen,
                enSrcPixelType = stFrameInfo.enPixelType,
                enDstPixelType = enDstPixelType,
                pDstBuffer = pImage,
                nDstBufferSize = _buffSizeForSaveImage
            };

            nRet = _myCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (MyCamera.MV_OK != nRet)
            {
                throw new InvalidOperationException("Unable to convert pixel type");
            }

            if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            {
                //************************Mono8 转 Bitmap*******************************
                output = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1,
                    PixelFormat.Format8bppIndexed, pImage);

                ColorPalette cp = output.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }

                output.Palette = cp;
            }
            else
            {
                //*********************RGB8 转 Bitmap**************************
                for (int i = 0; i < stFrameInfo.nHeight; i++)
                {
                    for (int j = 0; j < stFrameInfo.nWidth; j++)
                    {
                        byte chRed = _bufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3];
                        _bufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3] =
                            _bufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                        _bufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                    }
                }

                output = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3,
                    PixelFormat.Format24bppRgb, pImage);
            }

            return output;
        }

        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }


        private bool IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }


        #region Properties

        /// <summary>
        /// Friendly name or the factory name of the camera
        /// </summary>
        public string Name { get; set; }



        /// <summary>
        /// Internal handle of the camera
        /// </summary>
        internal MyCamera.MV_CC_DEVICE_INFO CameraInfo
        {
            get { return _cameraInfo; }
            set { _cameraInfo = value; }
        }

        /// <summary>
        /// Toggle grab start and stop
        /// </summary>
        public bool IsGrabbing
        {
            get { return _isGrabbing; }
            set
            {
                _isGrabbing = value;
                // Do not do anything meaningful before construction finished
                if (value) GrabStart();
                else GrabStop();
            }
        }

        /// <summary>
        /// Trigger source
        /// </summary>
        public CameraTriggerSourceType TriggerSourceType
        {
            get { return _triggerSourceType; }
            set
            {
                if (value == CameraTriggerSourceType.None) return;
                _triggerSourceType = value;
                uint flag = (uint) (value == CameraTriggerSourceType.Line0 ? 0 : 7);
                _myCamera.MV_CC_SetEnumValue_NET("TriggerSource", flag);
            }
        }

        /// <summary>
        /// True to realtime mode
        /// False to trigger mode
        /// </summary>
        public bool RealTimeMode    
        {
            get { return _realTimeMode; }
            set
            {
                _realTimeMode = value;
                uint flag = (uint) (value ? 0: 1);
                _myCamera.MV_CC_SetEnumValue_NET("TriggerMode", flag);
                if (value) return;
                // If realtime is off, reactivate trigger source
                ReactivateTriggerSource();
            }
        }

        public string IpAddress { get; set; }

        public CameraType CameraType { get; set; }

        public HObject ImageDisplay { get; set; }

        /// <summary>
        /// Toggle open close camera
        /// </summary>
        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                _isOpened = value;
                // Would not do anything meaningful before constructor finished
                if (value) Open();
                else
                {
                    IsGrabbing = false;
                    Close();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SoftwareExecuteOnceCommand { get; }
        public ICommand OpenCameraCommand { get; }
        public ICommand CloseCameraCommand { get; }
        public ICommand GrabStartCommand { get; }
        public ICommand GrabStopCommand { get; }
        public ICommand ToggleTriggerSourceCommand { get; }

        #endregion


        #region Command Execution

        public void SoftwareExecuteOnce()
        {
            var ret = _myCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (ret != 0)
            {
                Log("software trigger failed");
            }
        }

        private void ToggleTriggerSource(string s) {
            TriggerSourceType = s.ToCameraTriggerSourceType();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open the camera and hook up image acquisition call back
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            // Create
            var ret = _myCamera.MV_CC_CreateDevice_NET(ref _cameraInfo);
            if (ret != 0)
            {
                Log("Unable to create camera");
                return false;
            }

            // Open
            ret = _myCamera.MV_CC_OpenDevice_NET();
            if (ret != 0)
            {
                _myCamera.MV_CC_DestroyDevice_NET();
                Log("Unable to open camera");
                return false;
            }

            // Register image acquisition call back
            _imageCallbackDelegate = ImageCallback;
            ret = _myCamera.MV_CC_RegisterImageCallBackEx_NET(_imageCallbackDelegate, IntPtr.Zero);
            if (ret != 0)
            {
                Log("Register image acquisition call back failed");
                _myCamera.MV_CC_DestroyDevice_NET();
                return false;
            }


            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
            if (CameraType == CameraType.Gige)
            {
                int optimalSize = _myCamera.MV_CC_GetOptimalPacketSize_NET();
                if (optimalSize > 0)
                {
                    ret = _myCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint) optimalSize);
                    if (ret != MyCamera.MV_OK)
                    {
                        Log("Set packet size failed");
                    }
                }
                else
                {
                    Log("Get packet size failed");
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            _myCamera.MV_CC_SetEnumValue_NET("AcquisitionMode",
                2); // ch:工作在连续模式 | en:Acquisition On Continuous Mode  0:SingleFrame单帧 1:MultiFrame多帧  2:Continuous连续
            _myCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0); // ch:连续模式 | en:Continuous      0：Off  1：On

            // Reserve buffer
            _buffer = new byte[_bufferSize];
            _bufForSaveImage = new byte[_buffSizeForSaveImage];

            return true;
        }


        /// <summary>
        /// Start grabbing images
        /// </summary>
        /// <returns>Whether the operation is successful</returns>
        public bool GrabStart()
        {
            // Set default state after grabbing starts
            // Turn off real-time mode which is default
            // 0: real-time
            // 1: trigger
            RealTimeMode = false;
            ReactivateTriggerSource();
            
            var success =  _myCamera.MV_CC_StartGrabbing_NET() == 0;
            if(!success) Log("Grab start failed");
            
            return success;
        }

        /// <summary>
        /// Stop grabbing images
        /// </summary>
        /// <returns>Whether the operation is successful</returns>
        public bool GrabStop()
        {
            var success =  _myCamera.MV_CC_StopGrabbing_NET() == 0;
            if(!success) Log("Grab stop failed");
            return success;
        }


        /// <summary>
        /// Close the camera
        /// </summary>
        public void Close()
        {
            var nRet = _myCamera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet) return;

            nRet = _myCamera.MV_CC_DestroyDevice_NET();
        }

        #endregion
    }
}