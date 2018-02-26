using Upp.Net.Platform;
using Xunit;

namespace Upp.Net.IntegrationTests
{
    public class ClientTests
    {
        [Fact]
        public void StopsThreadWhenDisposeIsCalled()
        {
            var log = new MemoryTrace();
            var client = new Client(new IpEndpoint(IpAddress.LoopbackAddress, 5000), log);
            Assert.Equal(0, log.Count);
            client.Start();                        
            Assert.True(Wait.UntilTrue(
                () => log.GetTrace().Contains(MessageLoop<ListenerBase>.StartingLoopMessage)), log.ToString());
            client.Dispose();
            Assert.True(Wait.UntilTrue(
                () => log.GetTrace().Contains(MessageLoop<ListenerBase>.StoppingLoopMessage)), log.ToString());
        }
    }
}