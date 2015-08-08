using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface IPacket
    {
        UInt32 HeaderSize { get; }
        UInt32 DataSize { get; }
        byte Version { get; }
        byte[] Data { get; }
    }

    public class Packet : IPacket
    {
        public Packet(uint headerSize, uint dataSize, byte version, byte[] data)
        {
            HeaderSize = headerSize;
            DataSize = dataSize;
            Version = version;
            Data = data;
        }

        public UInt32 HeaderSize { get; private set; }
        public UInt32 DataSize { get; private set; }
        public byte Version { get; private set; }
        public byte[] Data { get; private set; }
    }
}
