using Windows.Devices.Gpio;

namespace Anheledir.NET.UWP.ShiftRegister
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

    public static GpioPinValue ToGpioPinValue(this bool value)
    {
      return value ? GpioPinValue.High : GpioPinValue.Low;
    }

    public static GpioPinValue ToGpioPinValue(this int value)
    {
      return value == 1 ? GpioPinValue.High : GpioPinValue.Low;
    }
  }
}