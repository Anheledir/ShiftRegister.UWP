using Windows.Devices.Gpio;

namespace Anheledir.NET.UWP.IoT
{
  public static class GpioHelper
  {
    public static bool ToBoolean(this GpioPinValue value)
    {
      return value == GpioPinValue.High;
    }

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