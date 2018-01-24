namespace Upp.Net.Platform
{
    public interface IUdpSend
    {
        void Send(byte[] buffer, int offset, int count);
    }
}
