## Dependencies
1. Reference MvCameraControl.Net.dll, halconDotNet.dll in both projects
2. halcon.dll and hcanvas.dll should be present in the UI project's binary directory

## Basic workflow
1. Call `HKCameraManager.ScanForAttachedCameras` and all attached cameras will be available via `HKCameraManager.AttachedCameras`
2. The class `CameraViewModel` asbstract functionality of a HKCamera and provide 4 major APIs:
```CS
var camera1 = HKCameraManager.AttachedCameras[0];

camera1.ImageAcquired += image =>{
    //Your image processing logic here
};
// Open the camera
camera1.Open();
// Start grabbing
camera1.GrabStart();
// Stop grabbing
camera1.GrabStop();
// Close camera
camera1.Close();

```
3. Real time mode and trigger source demo
```cs
// Turn off continuous mode
camera1.RealTimeMode = false;
// Set trigger source to software
camera1.TriggerSourceType = CameraTriggerSourceType.Software
// Software trigger
camera1.SoftwareExecuteOnce();

```

## For WPF users that follow MVVM pattern, the following bindings help you manage all the camera logics
1. Command bindings
```cs
  public ICommand SoftwareExecuteOnceCommand { get; }
  public ICommand OpenCameraCommand { get; }
  public ICommand CloseCameraCommand { get; }
  public ICommand GrabStartCommand { get; }
  public ICommand GrabStopCommand { get; }
  public ICommand ToggleTriggerSourceCommand { get; }

```

2. Property bindings
```cs

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

```