using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OneCog.Io.Onkyo
{
    public interface IPacketFactory
    {
        byte[] CreateIscpBuffer(UnitType unitType, string command);
        Stream CreateIscpStream(UnitType unitType, string command);
        Stream CreateIscpStream(UnitType unitType, IEnumerable<string> commands);
        IPacket CreatePacket(UnitType unitType, string command);
        string ExtractBody(IPacket packet);
    }

    public class PacketFactory : IPacketFactory
    {
        public static readonly IPacketFactory Default = new PacketFactory(true, new byte[] { 0x0D });
        public static readonly IPacketFactory Old = new PacketFactory(true, new byte[] { 0x1A, 0x0D, 0x0A });

        private static readonly Encoding Encoding = Encoding.UTF8;
        private static readonly byte[] IscpHeader = Encoding.GetBytes("ISCP");
        private static readonly byte Version = 1;
        private static readonly byte[] PaddedVersion = new byte[] { Version, 0, 0, 0 };
        private static readonly string CommandPattern = "!{0}{1}";
                
        private readonly bool _requiresFullDataLength;
        private readonly byte[] _commandSuffix;

        public PacketFactory(bool requiresFullDataLength, byte[] commandSuffix)
        {
            _requiresFullDataLength = requiresFullDataLength;
            _commandSuffix = commandSuffix;
        }

        private IEnumerable<byte> CreatePacketData(UnitType unitType, string command)
        {
            string formattedCommand = string.Format(CommandPattern, ((uint)unitType).ToString(), command);

            byte[] header = new byte[16];
            byte[] data = Encoding.GetBytes(formattedCommand).Concat(_commandSuffix).ToArray();

            int dataSize = _requiresFullDataLength ? data.Length + 6 : data.Length;

            Array.Copy(Encoding.GetBytes("ISCP"), 0, header, 0, 4);
            Array.Copy(BitConverter.BigEndian.GetBytes((UInt32)16), 0, header, 4, 4);
            Array.Copy(BitConverter.BigEndian.GetBytes((UInt32)dataSize), 0, header, 8, 4);
            Array.Copy(PaddedVersion, 0, header, 12, 4);

            return header.Concat(data);
        }

        public byte[] CreateIscpBuffer(UnitType unitType, string command)
        {
            return CreatePacketData(unitType, command).ToArray();
        }

        public Stream CreateIscpStream(UnitType unitType, string command)
        {
            return CreatePacketData(unitType, command).AsStream();
        }

        public Stream CreateIscpStream(UnitType unitType, IEnumerable<string> commands)
        {
            return commands.SelectMany(command => CreatePacketData(unitType, command)).AsStream();
        }
        
        public IPacket CreatePacket(UnitType unitType, string command)
        {
            byte[] buffer = CreateIscpBuffer(unitType, command);

            return new Packet(16, (uint) buffer.Length, 1, buffer);
        }
        
        public string ExtractBody(IPacket packet)
        {
            return Encoding.GetString(packet.Data, (int) packet.HeaderSize, (int) packet.DataSize - (int) packet.HeaderSize);
        }
    }
}
