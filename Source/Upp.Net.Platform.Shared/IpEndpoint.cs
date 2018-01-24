namespace Upp.Net.Platform
{
    public class IpEndpoint
    {
        public IpAddress IpAddress { get; }
        public int Port { get; }

        public IpEndpoint(IpAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public override string ToString()
        {
            return $"{nameof(IpAddress)}: {IpAddress}, {nameof(Port)}: {Port}";
        }

        protected bool Equals(IpEndpoint other)
        {
            return Equals(IpAddress, other.IpAddress) && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IpEndpoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((IpAddress?.GetHashCode() ?? 0) * 397) ^ Port;
            }
        }

        public static bool operator ==(IpEndpoint left, IpEndpoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IpEndpoint left, IpEndpoint right)
        {
            return !Equals(left, right);
        }
    }
}