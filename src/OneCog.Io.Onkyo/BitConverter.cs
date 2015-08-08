using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysBitConverter = System.BitConverter;

namespace OneCog.Io.Onkyo
{
    public class BitConverter
    {
        private enum Endian
        {
            Big,
            Little
        };

        public static readonly BitConverter BigEndian = new BitConverter(Endian.Big);

        public static readonly BitConverter LittleEndian = new BitConverter(Endian.Little);
        
        private Endian _endian;

        private BitConverter(Endian endian)
        {
            _endian = endian;
        }

        private bool Reverse
        {
            get { return SysBitConverter.IsLittleEndian && _endian != Endian.Little; }
        }

        private byte[] ToEndian(byte[] source)
        {
            return Reverse ? source.Reverse().ToArray() : source;
        }

        public byte[] GetBytes(uint value)
        {
            return ToEndian(SysBitConverter.GetBytes(value)); 
        }

        internal uint ToUInt32(byte[] source, int offset)
        {
            byte[] buffer = new byte[4];

            Array.Copy(source, offset, buffer, 0, 4);

            buffer = ToEndian(buffer);

            return SysBitConverter.ToUInt32(buffer, 0);
        }
    }
}
