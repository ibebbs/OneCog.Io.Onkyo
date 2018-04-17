using NUnit.Framework;
using OneCog.Io.Onkyo.Devices;
using OneCog.Io.Onkyo.Messages;
using OneCog.Io.Onkyo.Messages.Zone1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneCog.Io.Onkyo.Tests.Messages
{
    [TestFixture]
    public class AbstractParserTestFixture
    {
        private AbstractParser _subject;

        [OneTimeSetUp]
        public void Setup()
        {
            _subject = new AbstractParser(Tx.AllParsers);
        }

        [Test]
        public void CanParsePowerOnCommandResponse()
        {
            IEnumerable<IResponse> responses = _subject.Parse("!PWR01");

            PowerStateResponse response = responses.OfType<PowerStateResponse>().SingleOrDefault(psr => psr.PowerState == Common.PowerState.On);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public void CanParsePowerOffCommandResponse()
        {
            IEnumerable<IResponse> responses = _subject.Parse("!PWR00");

            PowerStateResponse response = responses.OfType<PowerStateResponse>().SingleOrDefault(psr => psr.PowerState == Common.PowerState.Off);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public void CanParseVolumeCommandResponse()
        {
            IEnumerable<IResponse> responses = _subject.Parse("!MVL5A");

            VolumeResponse response = responses.OfType<VolumeResponse>().SingleOrDefault(vr => vr.Volume == 90);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public void CanParseInputSourceResponse()
        {
            IEnumerable<IResponse> responses = _subject.Parse("!SLI2B");

            InputSourceResponse response = responses.OfType<InputSourceResponse>().SingleOrDefault(vr => string.Equals(vr.InputSource.Name, "NETWORK", StringComparison.CurrentCultureIgnoreCase));

            Assert.That(response, Is.Not.Null);
        }
    }
}
