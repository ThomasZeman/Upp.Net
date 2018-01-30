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
            Assert.Equal(0, log.List.Count);
            listener.Start();
            Assert.Contains(MessageLoop<ListenerBase>.StartingLoopMessage, log.List);
            var preCount = log.List.Count;
            listener.Dispose();
            Assert.True(Wait.UntilTrue(() => log.List.Count == preCount + 1));
            Assert.Contains(MessageLoop<ListenerBase>.StoppingLoopMessage, log.List);
        }
    }
}
