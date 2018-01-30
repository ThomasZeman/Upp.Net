using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Upp.Net.Platform;
using Upp.Net.Serialization;
using Upp.Net.Trace;
using Xunit;

namespace Upp.Net.IntegrationTests
{
    public class ServerDiscoveryTests
    {
        [Fact]
        public void SendDiscoveryRequest_TwoServersRespond()
        {
            // This test should test with more than one server but that would mean to have more local ips
            // to bind to
            var ips = GetAllIpv4Addresses().ToList();
            var closetEndpoints = FindMostPopulatedSubnet(ips);
            if (closetEndpoints.Count <= 1)
            {
                Assert.True(false, "System does not have more than 1 IP in the same 255.255.255.0 subnet to test with");
            }
            var received = new List<Tuple<ServerDiscoveryResponse, IpEndpoint>>();
            using (var serverDiscovery = new BroadcastClient(new IpEndpoint(closetEndpoints.First(), 4000), 4338, 3, new NullTrace()))
            {
                var serializer = new Serializer<ISerializableMessage>();
                serializer.AddType<ServerDiscoveryResponse>();
                serializer.AddType<ServerDiscoveryRequest>();
                var typedConnection = new TypedConnection<ISerializableMessage>(serverDiscovery.Connection, serializer);
                typedConnection.NewMessage += (p1, p2, p3) =>
                {
                    received.Add(Tuple.Create((ServerDiscoveryResponse)p2, serverDiscovery.CurrentEndpoint));
                };
                serverDiscovery.StartReceiving();
                var amountConnections = new int[1];
                var servers = new List<Server>();

                ips = closetEndpoints;
                int amountOfServer = ips.Count;
                for (int i = 0; i < amountOfServer; i++)
                {
                    var copyOfi = i + 1;
                    var server = new Server(new ServerConfiguration(new IpEndpoint(ips[i], 4338)), new NullTrace());
                    server.NewServerPeer += (server1, serverPeer) =>
                    {
                        serverPeer.NewConnection += (p1, p2) =>
                        {
                            amountConnections[0]++;
                            int runningId = 0;
                            var typed = new TypedConnection<ISerializableMessage>(p2, serializer);
                            typed.NewMessage += (p3, p4, p5) =>
                            {
                                var request = ((ServerDiscoveryRequest) p4);
                                var response = new ServerDiscoveryResponse {ServerVersion = request.ClientVersion * copyOfi, RunningId = runningId++};
                                typed.Send(response);
                            };
                        };
                    };
                    servers.Add(server);
                    server.StartReceiving();
                }
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var receivedBefore = received.Count;
                        typedConnection.Send(new ServerDiscoveryRequest() { ClientVersion = i });
                        System.Threading.Thread.Sleep(1);
                        Assert.True(Wait.UntilTrue(() => receivedBefore + amountOfServer == received.Count));
                    }
                    Assert.Equal(amountOfServer * 100, received.Count);
                    for (var index = 0; index < ips.Count; index++)
                    {
                        var ipAddress = ips[index];
                        var byIp = received.Where(_ => _.Item2.IpAddress.Equals(ipAddress)).ToList();
                        Assert.Equal(100, byIp.Count);
                        byIp.Sort((p1, p2) => p1.Item1.ServerVersion.CompareTo(p2.Item1.ServerVersion));
                        Assert.Equal(Enumerable.Range(0, 100).Select(_ => _ * (1 + index)), byIp.Select(_ => _.Item1.ServerVersion));
                    }
                }
                finally
                {
                    foreach (var server in servers)
                    {
                        server.Dispose();
                    }
                }
            }
        }

        private static List<IpAddress> FindMostPopulatedSubnet(List<IpAddress> ips)
        {
            List<IpAddress> closetEndpoints = null;
            int highestRank = 0;
            Func<uint, uint> getThreeLeastSigBytes = _ => _ & (((uint)(1 << 24) - 1));
            foreach (var ipAddress in ips)
            {
                uint subnet = getThreeLeastSigBytes(ipAddress.Ipv4Address);
                var closest = ips.Where(_ => getThreeLeastSigBytes(_.Ipv4Address) == subnet).ToList();
                if (closest.Count() > highestRank)
                {
                    highestRank = closest.Count();
                    closetEndpoints = closest;
                }
            }
            Assert.NotNull(closetEndpoints);
            return closetEndpoints;
        }

        public IEnumerable<IpAddress> GetAllIpv4Addresses()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            yield return new IpAddress((uint)ip.Address.Address);
                        }
                    }
                }
            }
        }
    }

    public class ServerDiscoveryResponse : ISerializableMessage
    {
        public byte TypeId => 2;
        public string ServerName;
        public int ServerVersion;
        public int RunningId;

        public void Serialize(Paket paket)
        {
            SimpleTypeWriter.Write(ServerName, paket);
            SimpleTypeWriter.Write(ServerVersion, paket);
            SimpleTypeWriter.Write(RunningId, paket);
        }

        public void Deserialize(Paket paket)
        {
            ServerName = SimpleTypeReader.ReadString(paket);
            ServerVersion = SimpleTypeReader.ReadInt(paket);
            RunningId = SimpleTypeReader.ReadInt(paket);
        }

        public override string ToString()
        {
            return $"ServerName: {ServerName}, ServerVersion: {ServerVersion}";
        }
    }

    public class ServerDiscoveryRequest : ISerializableMessage
    {
        public byte TypeId => 1;
        public int ClientVersion;

        public void Serialize(Paket paket)
        {
            SimpleTypeWriter.Write(ClientVersion, paket);
        }

        public void Deserialize(Paket paket)
        {
            ClientVersion = SimpleTypeReader.ReadInt(paket);
        }

        public override string ToString()
        {
            return $"ClientVersion: {ClientVersion}";
        }
    }
}
