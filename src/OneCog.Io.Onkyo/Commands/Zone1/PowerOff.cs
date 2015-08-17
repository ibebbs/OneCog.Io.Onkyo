﻿using OneCog.Io.Onkyo.Common;
using OneCog.Io.Onkyo.Responses.Zone1;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Commands.Zone1
{
    public class PowerOff : ICommand<PowerState>
    {
        private const string CommandString = "PWR00";
        
        public Task<Fallible<PowerState>> Send(ICommandStream stream)
        {
            return stream.Send<PowerState>(CommandString, 
                response => response
                    .OfType<PowerStateResponse>()
                    .Where(r => r.PowerState == PowerState.Off)
                    .Select(r => r.PowerState)
            );
        }
    }
}