using Anheledir.NET.UWP.ShiftRegister;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace ShiftRegister.UWP
{
  public class ShiftRegister
  {
    // Serial Data input
    private byte _ds;
    private GpioPin _DS;

    // Master re-clear
    private GpioPin _MR;
    private byte _mr;

    // Output Enable
    private GpioPin _OE;
    private byte _oe;

    // Shift Register Clock-Pin
    private GpioPin _SH_CP;
    private byte _shCP;

    // Storage Register Clock Pin (latch pin)
    private GpioPin _ST_CP;
    private byte _stCP;

    private int _registerAmount;
    private int[] _shiftRegisters;
    private bool _isOutputEnabled;

    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP) : this(DS, ST_CP, SH_CP, 1)
    {
      Debug.WriteLine("Using default for numberOfRegisters = 1");
    }

    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP, int numberOfRegisters) : this(DS, ST_CP, SH_CP, numberOfRegisters, 0, 0)
    {
      Debug.WriteLine("Using default for OE = 0 and MR = 0");
    }

    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP, int numberOfRegisters, byte OE, byte MR)
    {
      _ds = DS;
      _stCP = ST_CP;
      _shCP = SH_CP;
      _oe = OE;
      _mr = MR;

      _registerAmount = numberOfRegisters;
      _shiftRegisters = new int[_registerAmount];

      _isOutputEnabled = _oe == 0;

      InitGPIO();
      ResetPins();
      Commit();
    }

    /// <summary>
    /// Set and display registers
    /// Only call AFTER all values are set how you would like (slow otherwise)
    /// </summary>
    public void Commit()
    {
      _ST_CP.Write(GpioPinValue.Low);

      for (int i = _registerAmount - 1; i >= 0; i--)
      {
        for (int j = 8 - 1; j >= 0; j--)
        {
          _SH_CP.Write(GpioPinValue.Low);
          int pinValue = _shiftRegisters[i] & (1 << j);
          _DS.Write(pinValue.ToGpioPinValue());
          _SH_CP.Write(GpioPinValue.High);
        }
      }

      _ST_CP.Write(GpioPinValue.High);
    }

    public async Task MasterReset()
    {
      if (_MR != null)
      {
        _MR.Write(GpioPinValue.Low);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        _MR.Write(GpioPinValue.High);
      } else
      {
        Debug.WriteLine("GPIO for Master Re-Clear not initialized");
      }
    }

    public void ResetPins()
    {
      SetAll(GpioPinValue.Low);
    }

    public bool IsOutputEnabled
    {
      get { return _isOutputEnabled; }
      set
      {
        if (_isOutputEnabled != value && _OE != null)
        {
          _OE.Write(value.ToGpioPinValue(true));
          _isOutputEnabled = value;
        }
        else if (_OE == null)
        {
          Debug.WriteLine("GPIO for Output Enabled not initialized");
        }
      }
    }

    public void SetAll(GpioPinValue value)
    {
      for (int i = _registerAmount * 8 - 1; i >= 0; i--)
      {
        SetPin(i, value);
      }
    }

    public void SetPin(int index, GpioPinValue newValue)
    {
      int byteIndex = index / 8;
      int bitIndex = index % 8;

      int currentRegister = _shiftRegisters[byteIndex];

      currentRegister &= ~(1 << bitIndex);
      currentRegister |= newValue.ToByte() << bitIndex;

      _shiftRegisters[byteIndex] = currentRegister;
    }

    private void InitGPIO()
    {
      var gpio = GpioController.GetDefault();

      if (gpio == null)
      {
        _DS = null;
        _SH_CP = null;
        _ST_CP = null;
        _OE = null;
        _MR = null;

        throw new NoGPIOControllerFoundException("There is no GPIO controller on this device.");
      }

      _DS = gpio.OpenPin(_ds);
      _DS.Write(GpioPinValue.Low);
      _DS.SetDriveMode(GpioPinDriveMode.Output);

      _SH_CP = gpio.OpenPin(_shCP);
      _SH_CP.Write(GpioPinValue.Low);
      _SH_CP.SetDriveMode(GpioPinDriveMode.Output);

      _ST_CP = gpio.OpenPin(_stCP);
      _ST_CP.Write(GpioPinValue.Low);
      _ST_CP.SetDriveMode(GpioPinDriveMode.Output);

      if (_oe > 0)
      {
        _OE = gpio.OpenPin(_oe);
        _OE.Write(GpioPinValue.High);
        _OE.SetDriveMode(GpioPinDriveMode.Output);
      }
      else
      {
        _OE = null;
        Debug.WriteLine("GPIO for Output Enabled not initialized");
      }

      if (_mr > 0)
      {
        _MR = gpio.OpenPin(_mr);
        _MR.Write(GpioPinValue.High);
        _MR.SetDriveMode(GpioPinDriveMode.Output);
      }
      else
      {
        _MR = null;
        Debug.WriteLine("GPIO for Master Re-Clear not initialized");
      }

      Debug.WriteLine("GPIO pins initialized");
    }
  }
}