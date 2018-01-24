namespace Upp.Net.Platform
{
    public class UdpSocketException : System.Exception
    {
        public int ErrorCode { get; }

        public UdpSocketException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}