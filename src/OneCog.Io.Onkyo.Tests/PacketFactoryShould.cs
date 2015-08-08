using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Tests
{
    [TestFixture]
    public class PacketFactoryShould
    {
        [Test]
        public void CreateValidIscpPacket()
        {
            byte[] expected = new byte [] {
                (byte)'I', (byte)'S', (byte)'C', (byte)'P',
                16, 0, 0, 0,
                24, 0, 0, 0,
                1, 0, 0, 0,
                (byte)'!', (byte)'1',(byte)'P',(byte)'W',
                (byte)'R', (byte)'0', (byte)'1', 16
            };

            byte[] actual = PacketFactory.Default.CreateIscpBuffer(UnitType.Receiver, "PWR01");

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
