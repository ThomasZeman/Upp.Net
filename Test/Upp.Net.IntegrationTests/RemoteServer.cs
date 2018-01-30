using System;
using System.IO;
using System.Threading;
using Upp.Net.Serialization;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    public class RemoteServer
    {
        public static ManualResetEvent StopEvent = new ManualResetEvent(false);

        public static void Run(int port, ITrace trace)
        {
            File.Delete("RemoteHost.txt");
            using (TestOutputFile =
                new StreamWriter(
                    File.Open("RemoteHost.txt", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None)))
            {
                var serializer = new Serializer<ISerializableMessage>();
                serializer.AddType<TestData>();
                serializer.AddType<TestControl>();
                var serverConfiguration = new ServerConfiguration(port);
                var server = new Net.Server(serverConfiguration, trace);
                Console.WriteLine("Starting server");
                server.NewServerPeer += (server1, serverPeer) =>
                {
                    serverPeer.NewConnection += (serverPeer1, connection) => new ServerConnection(serializer, trace).Server_NewConnection(server1, connection);
                };
                server.StartReceiving();
                Console.WriteLine("Setting signal that we are up and running");
                using (var startEvent = EventWaitHandle.OpenExisting("testRemoteServer"))
                {
                    startEvent.Set();
                }
                Console.WriteLine("Waiting for stop events");
                StopEvent.WaitOne(600000);
                server.Dispose();
            }
        }

        public static StreamWriter TestOutputFile { get; set; }
    }
}