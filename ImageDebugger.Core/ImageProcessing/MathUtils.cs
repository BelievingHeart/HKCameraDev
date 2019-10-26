using System;

namespace ImageDebugger.Core.ImageProcessing
{
    public static class MathUtils
    {
        public static double ToRadian(double degree)
        {
            return degree / 180.0 * Math.PI;
        }
    }
}
