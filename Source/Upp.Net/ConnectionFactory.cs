using System;
using Upp.Net.Platform;
using Upp.Net.Trace;

namespace Upp.Net
{
    public static class ConnectionFactory
    {
        public static Connection Create(ServiceTypes serviceType, int connectionId, IUdpSend udpSend, ITrace trace)
        {
            switch (serviceType)
            {
                case ServiceTypes.UnreliableUnordered:
                    return new UnreliableUnorderedConnection(udpSend, (byte) connectionId, new TracePrefixFacade(connectionId + " UU ", trace));
                case ServiceTypes.UnreliableOrdered:
                    return new UnreliableOrderedConnection(udpSend, (byte) connectionId, new TracePrefixFacade(connectionId + " UO ", trace));
                case ServiceTypes.ReliableOrdered:
                    return new ReliableOrderedConnection(udpSend, (byte)connectionId, new TracePrefixFacade(connectionId + " RO ", trace));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}