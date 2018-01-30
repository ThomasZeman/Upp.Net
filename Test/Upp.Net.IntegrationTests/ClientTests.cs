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
            Assert.Equal(0, log.List.Count);
            client.Start();
            Assert.Contains(MessageLoop<Client>.StartingLoopMessage, log.List);
            var preCount = log.List.Count;
            client.Dispose();
            Assert.True(Wait.UntilTrue(() => log.List.Count == preCount + 1));
            Assert.Contains(MessageLoop<Client>.StoppingLoopMessage, log.List);
        }
    }
}