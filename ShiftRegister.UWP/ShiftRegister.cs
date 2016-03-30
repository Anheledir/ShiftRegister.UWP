using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Anheledir.NET.UWP.IoT
{
  public class ShiftRegister
  {
    /// <summary>
    /// GPIO-Pin number for Serial Data Input
    /// </summary>
    public byte DS { get; internal set; }
    private GpioPin _DS;

    /// <summary>
    /// GPIO-Pin for Master Re-Clear
    /// </summary>
    public byte MR { get; internal set; }
    private GpioPin _MR;

    /// <summary>
    /// GPIO-Pin for Output Enable
    /// </summary>
    public byte OE { get; internal set; }
    private GpioPin _OE;

    /// <summary>
    /// GPIO-Pin for the Shift Register Clock
    /// </summary>
    public byte SH_CP { get; internal set; }
    private GpioPin _SH_CP;

    /// <summary>
    /// GPIO-Pin for the Storage Register Clock (latch)
    /// </summary>
    public byte ST_CP { get; internal set; }
    private GpioPin _ST_CP;

    public int RegisterAmount { get; internal set; }
    private int[] _shiftRegisters;
    private bool _isOutputEnabled;

    /// <summary>
    /// Initialize a new Instance of the Shift Register Library to work with only one Register
    /// You should connect OE to GND and MR to Vcc (positive supply voltage)
    /// </summary>
    /// <param name="DS">Pin for Serial Data input</param>
    /// <param name="ST_CP">Pin for Storage Register Clock (latch)</param>
    /// <param name="SH_CP">Pin for Shift Register Clock</param>
    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP) : this(DS, ST_CP, SH_CP, 1)
    {
      Debug.WriteLine("Using default for numberOfRegisters = 1");
    }

    /// <summary>
    /// Initialize a new Instance of the Shift Register Library to work with more than one Register
    /// You should connect OE to GND and MR to Vcc (positive supply voltage)
    /// </summary>
    /// <param name="DS">Pin for Serial Data input</param>
    /// <param name="ST_CP">Pin for Storage Register Clock (latch)</param>
    /// <param name="SH_CP">Pin for Shift Register Clock</param>
    /// <param name="numberOfRegisters">Number of serial-connected shift registers</param>
    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP, int numberOfRegisters) : this(DS, ST_CP, SH_CP, numberOfRegisters, 0, 0)
    {
      Debug.WriteLine("Using default for OE = 0 and MR = 0");
    }

    /// <summary>
    /// Initialize a new Instance of the Shift Register Library to work with more than one Register
    /// </summary>
    /// <param name="DS">Pin for Serial Data input</param>
    /// <param name="ST_CP">Pin for Storage Register Clock (latch)</param>
    /// <param name="SH_CP">Pin for Shift Register Clock</param>
    /// <param name="numberOfRegisters">Number of serial-connected shift registers</param>
    /// <param name="OE">Pin for Output Enabled (active Low)</param>
    /// <param name="MR">Pin for Master Re-Clear (active Low)</param>
    public ShiftRegister(byte DS, byte ST_CP, byte SH_CP, int numberOfRegisters, byte OE, byte MR)
    {
      this.DS = DS;
      this.ST_CP = ST_CP;
      this.SH_CP = SH_CP;
      this.OE = OE;
      this.MR = MR;

      RegisterAmount = numberOfRegisters;
      _shiftRegisters = new int[RegisterAmount];

      _isOutputEnabled = this.OE == 0;

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

      for (int i = RegisterAmount - 1; i >= 0; i--)
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

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
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
      for (int i = RegisterAmount * 8 - 1; i >= 0; i--)
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

      _DS = gpio.OpenPin(DS);
      _DS.Write(GpioPinValue.Low);
      _DS.SetDriveMode(GpioPinDriveMode.Output);

      _SH_CP = gpio.OpenPin(SH_CP);
      _SH_CP.Write(GpioPinValue.Low);
      _SH_CP.SetDriveMode(GpioPinDriveMode.Output);

      _ST_CP = gpio.OpenPin(ST_CP);
      _ST_CP.Write(GpioPinValue.Low);
      _ST_CP.SetDriveMode(GpioPinDriveMode.Output);

      if (OE > 0)
      {
        _OE = gpio.OpenPin(OE);
        _OE.Write(GpioPinValue.High);
        _OE.SetDriveMode(GpioPinDriveMode.Output);
      }
      else
      {
        _OE = null;
        Debug.WriteLine("GPIO for Output Enabled not initialized");
      }

      if (MR > 0)
      {
        _MR = gpio.OpenPin(MR);
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