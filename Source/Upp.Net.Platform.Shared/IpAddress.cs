using System;
using System.Linq;

namespace Upp.Net.Platform
{
    public class IpAddress
    {
        private readonly uint _ipv4Address;

        public uint Ipv4Address => _ipv4Address;

        public static IpAddress BroadcastAddress => new IpAddress(new byte[] { 255, 255, 255, 255 });
        public static IpAddress AnyAddress => new IpAddress(new byte[] { 0, 0, 0, 0 });
        public static IpAddress LoopbackAddress => new IpAddress(new byte[] { 127, 0, 0, 1 });

        public IpAddress(uint ipv4Address)
        {
            _ipv4Address = ipv4Address;
        }

        public IpAddress(byte[] ipAddress)
        {
            if (ipAddress.Length == 4)
            {
                _ipv4Address = (uint)(ipAddress[0] | (ipAddress[1] << 8) | (ipAddress[2] << 16) | (ipAddress[3] << 24));
            }
            else
            {
                throw new ArgumentException("ipAddress does not contain 4 numbers");
            }
        }

        public IpAddress(string ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            var numbers = ipAddress.Split('.');
            if (numbers.Length != 4)
            {
                throw new ArgumentException($"{ipAddress} does not contain 4 numbers");
            }
            var list = numbers.Select(int.Parse).ToList();
            if (list.Any(_ => _ < 0 || _ > 255))
            {
                throw new ArgumentException($"{ipAddress} contains at least one invalid number");
            }
            _ipv4Address = (uint)(list[0] | (list[1] << 8) | (list[2] << 16) | (list[3] << 24));
        }

        public override string ToString()
        {
            var byte0 = (byte)(Ipv4Address & 255);
            var byte1 = (byte)(Ipv4Address >> 8);
            var byte2 = (byte)(Ipv4Address >> 16);
            var byte3 = (byte)(Ipv4Address >> 24);
            return $"{byte0}.{byte1}.{byte2}.{byte3}";
        }

        protected bool Equals(IpAddress other)
        {
            return _ipv4Address == other._ipv4Address;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IpAddress)obj);
        }

        public override int GetHashCode()
        {
            return (int)_ipv4Address;
        }

        public static bool operator ==(IpAddress left, IpAddress right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IpAddress left, IpAddress right)
        {
            return !Equals(left, right);
        }
    }
}
