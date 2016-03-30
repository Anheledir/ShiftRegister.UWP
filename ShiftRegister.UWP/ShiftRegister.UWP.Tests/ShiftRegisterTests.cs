using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Anheledir.NET.UWP.IoT.Tests
{
  [TestClass]
  public class ShiftRegisterTests
  {
    [TestMethod]
    public void SerialDataInputPinShouldBeCorrectlyInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(1, sr.DS);
    }

    [TestMethod]
    public void StorageRegisterClockPinShouldBeCorrectlyInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(2, sr.ST_CP);
    }

    [TestMethod]
    public void ShiftRegisterClockPinShouldBeCorrectlyInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(3, sr.SH_CP);
    }

    [TestMethod]
    public void NumberOfRegistersShouldBeDefaultValueWhenNotInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(1, sr.RegisterAmount);
    }

    [TestMethod]
    public void NumberOfRegistersShouldBeCorrectlyInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3, 4);
      Assert.AreEqual(4, sr.RegisterAmount);
    }

    [TestMethod]
    public void OutputEnabledPinShouldBeDefaultValueWhenNotInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(0, sr.OE);
    }

    [TestMethod]
    public void MasterReClearPinShouldBeDefaultWhenNotInitialized()
    {
      var sr = new ShiftRegister(1, 2, 3);
      Assert.AreEqual(0, sr.MR);
    }

  }
}