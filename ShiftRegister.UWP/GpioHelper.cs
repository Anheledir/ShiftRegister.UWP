using Windows.Devices.Gpio;

namespace Anheledir.NET.UWP.IoT
{
    /// <summary>
    /// This class contains a few utility extension methods to use with GPIO devices.
    /// </summary>
    public static class GpioHelper
    {
        /// <summary>
        /// Converts a <see>GpioPinValue</see> into a <see>bool</see>.
        /// </summary>
        /// <param name="value">The <see>GpioPinValue</see> to convert.</param>
        /// <returns><c>true</c> if <param>value</param> is High, otherwise <c>false</c>.</returns>
        public static bool ToBoolean(this GpioPinValue value)
        {
            return value == GpioPinValue.High;
        }

        /// <summary>
        /// Converts a <see>GpioPinValue</see> into a <see>byte</see>.
        /// </summary>
        /// <param name="value">The <see>GpioPinValue</see> to convert.</param>
        /// <returns><c>1</c> if <param>value</param> is High, otherwise <c>0</c>.</returns>
        public static byte ToByte(this GpioPinValue value)
        {
            return value == GpioPinValue.High ? (byte)1 : (byte)0;
        }

        public static GpioPinValue ToGpioPinValue(this bool value, bool trueIsLow = false)
        {
            return value ?
              trueIsLow ? GpioPinValue.Low : GpioPinValue.High
              : trueIsLow ? GpioPinValue.High : GpioPinValue.Low;
        }

        public static GpioPinValue ToGpioPinValue(this int value, bool oneIsLow = false)
        {
            return value == 1 ?
              oneIsLow ? GpioPinValue.Low : GpioPinValue.High
              : oneIsLow ? GpioPinValue.High : GpioPinValue.Low;
        }
    }
}