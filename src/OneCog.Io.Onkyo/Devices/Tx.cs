using OneCog.Io.Onkyo.Messages;
using System;
using System.Collections.Generic;

namespace OneCog.Io.Onkyo.Devices
{
    public static class Tx
    {
        internal static readonly IEnumerable<IParser> AllParsers = new IParser[]
        {
            new Messages.Zone1.PowerStateParser(),
            new Messages.Zone1.VolumeResponseParser(),
            new Messages.Zone1.MuteStateParser(),
            new Messages.Zone1.InputSourceResponseParser(),
            new Messages.Zone2.PowerStateParser(),
            new Messages.Zone2.VolumeResponseParser(),
            new Messages.Zone2.MuteStateParser(),
            new Messages.Zone2.InputSourceResponseParser()
        };

        public static IReceiver Nr929(string host, ushort port, TimeSpan commandTimeout)
        {
            return new Receiver(host, port, commandTimeout, AllParsers);
        }

        public static IReceiver Nr616(string host, ushort port, TimeSpan commandTimeout)
        {
            return new Receiver(host, port, commandTimeout, AllParsers);
        }
    }
}
