﻿using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    public class GetVolume : ICommand<byte>
    {
        private const string CommandString = "ZVLQSTN";

        public Task<Fallible<byte>> Send(ICommandStream stream)
        {
            return stream.Send<byte>(
                CommandString,
                responses => responses
                    .OfType<VolumeResponse>()
                    .Select(response => response.Volume)
            );
        }
    }
}
