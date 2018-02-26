using Upp.Net.Platform;
using Xunit;

namespace Upp.Net.IntegrationTests
{
    public class UdpListenerTests
    {
        [Fact]
        public void StopsListeningWhenDisposeIsCalled()
        {
            var log = new MemoryTrace();
            var listener = new ListenerBase(new IpEndpoint(IpAddress.AnyAddress, 3333), log);
            Assert.Equal(0, log.Count);
            listener.Start();
            Assert.True(Wait.UntilTrue(
                    () => log.GetTrace().Contains(MessageLoop<ListenerBase>.StartingLoopMessage)), log.ToString());
            listener.Dispose();
            Assert.True(Wait.UntilTrue(
                () => log.GetTrace().Contains(MessageLoop<ListenerBase>.StoppingLoopMessage)), log.ToString());
        }
    }
}