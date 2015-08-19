using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    public class MuteOnCommand : ICommand<MuteState>
    {
        private const string CommandString = "ZMT01";

        public Task<Fallible<MuteState>> Send(ICommandStream stream)
        {
            return stream.Send<MuteState>(CommandString,
                response => response
                    .OfType<MuteStateResponse>()
                    .Where(r => r.MuteState == MuteState.Muted)
                    .Select(r => r.MuteState)
            );
        }
    }
}
