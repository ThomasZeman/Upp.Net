using System;
using System.Threading;
using Upp.Net.Serialization;
using Upp.Net.Trace;

namespace Upp.Net.IntegrationTests
{
    class ServerConnection
    {
        private readonly Serializer<ISerializableMessage> _serializer;
        private readonly ITrace _trace;
        private TypedConnection<ISerializableMessage> _typedConnection;
        public static int _connectionIdCounter;
        public int _connectionId;

        public ServerConnection(Serializer<ISerializableMessage> serializer, ITrace trace)
        {
            _serializer = serializer;
            _trace = trace;
            _connectionId = Interlocked.Increment(ref _connectionIdCounter);
            Console.WriteLine("Created new ServerConnection: " + _connectionId);
        }

        public void Server_NewConnection(Net.Server arg1, Connection arg2)
        {
            _typedConnection = new TypedConnection<ISerializableMessage>(arg2, _serializer);
            _typedConnection.NewMessage += _typedConnection_NewMessage;
            //          _typedConnection.Send(new TestData(), ServiceTypes.ReliableOrdered);
        }

        private void _typedConnection_NewMessage(ITypedConnection<ISerializableMessage> arg1, ISerializableMessage arg2, ushort sequenceId)
        {
            if (arg2 is TestControl)
            {
                _trace.Info("Got Stop signal");
                RemoteServer.StopEvent.Set();
            }
            RemoteServer.TestOutputFile.WriteLine(_connectionId + ":" + arg2.ToString());
        }
    }
}