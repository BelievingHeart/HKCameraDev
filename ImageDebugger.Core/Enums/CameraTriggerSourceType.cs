﻿﻿namespace ImageDebugger.Core.Enums
{
    public enum CameraTriggerSourceType
    {
        Line0,
        Software,
        None
    }

    public static class CameraTriggerSourceTypeHelper
    {
        public static CameraTriggerSourceType ToCameraTriggerSourceType(this string s)
        {
            if(s == CameraTriggerSourceType.Line0.ToString())
            {
                return CameraTriggerSourceType.Line0;
            }
            
            return s == CameraTriggerSourceType.Software.ToString() ? CameraTriggerSourceType.Software : CameraTriggerSourceType.None;
        }
    }
}