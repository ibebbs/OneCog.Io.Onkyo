using NUnit.Framework;
using OneCog.Io.Onkyo.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Tests.Devices
{
    [TestFixture]
    public class TxTestFixture
    {
        [Test]
        public void CanConstructNr616()
        {
            IReceiver receiver = Tx.Nr616("127.0.0.1", 60128, TimeSpan.FromSeconds(1));

            Assert.That(receiver, Is.Not.Null);
        }

        [Test]
        public void CanConstructNr929()
        {
            IReceiver receiver = Tx.Nr929("127.0.0.1", 60128, TimeSpan.FromSeconds(1));

            Assert.That(receiver, Is.Not.Null);
        }
    }
}
