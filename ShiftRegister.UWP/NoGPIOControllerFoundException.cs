namespace Anheledir.NET.UWP.IoT
{
    /// <summary>
    /// This exception is thrown when trying to initialize <see>ShiftRegister</see> and no GPIO controller is available (e.g. not running on an IoT-Device).
    /// </summary>
    public class NoGPIOControllerFoundException : System.Exception
    {
        public NoGPIOControllerFoundException()
        {
        }

        public NoGPIOControllerFoundException(string message) : base(message)
        {
        }

        public NoGPIOControllerFoundException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}