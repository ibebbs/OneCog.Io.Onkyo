using System;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.TxNr
{
    public interface IDevice
    {
        Task<IPacket> SendAndReceiveAsync(IPacket packet, Predicate<IPacket> predicate);
    }

    public class Device : IDevice
    {
        private readonly IIscpStream _steam;

        public Device(string hostName, ushort port, UnitType unitType) : this (new IscpStream(hostName, port, unitType)) { }

        public Device(IIscpStream stream)
        {
            _steam = stream;
        }

        public Task<IPacket> SendAndReceiveAsync(IPacket packet, Predicate<IPacket> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
