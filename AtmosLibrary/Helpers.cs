using System;
using System.Collections.Generic;
using System.Text;

namespace AtmosLibrary
{
    class Helpers
    {
        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radians">duh</param>
        /// <returns></returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Converts degs to radians
        /// </summary>
        /// <param name="degrees">duh</param>
        /// <returns></returns>
        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        /// Clamps a num between a min and a max
        /// </summary>
        /// <param name="num">duh</param>
        /// <param name="min">duh</param>
        /// <param name="max">duh</param>
        /// <returns></returns>
        public static double Clamp(double num, double min, double max)
        {
            return (num < min ? min : (num > max ? max : num));
        }
    }
}
