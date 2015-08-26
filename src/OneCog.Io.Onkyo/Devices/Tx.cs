using OneCog.Io.Onkyo.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Devices
{
    public static class Tx
    {
        internal static readonly IEnumerable<IParser> AllParsers = new IParser[]
        {
            new Messages.Zone1.PowerStateParser(),
            new Messages.Zone1.VolumeResponseParser(),
            new Messages.Zone1.MuteStateParser(),
            new Messages.Zone2.PowerStateParser(),
            new Messages.Zone2.VolumeResponseParser(),
            new Messages.Zone2.MuteStateParser()
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
