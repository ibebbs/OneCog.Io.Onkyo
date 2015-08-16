﻿using OneCog.Io.Onkyo.Responses;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Commands
{
    public class SetMasterVolume : ICommand<byte>
    {
        private const string CommandStringPattern = "MVL{0:X2}";
        
        private byte _volume;

        public SetMasterVolume(byte volume)
        {
            if (volume > 100) throw new ArgumentException("Volume must be between 0 and 100", "volume");

            _volume = volume;
        }

        public Task<Fallible<byte>> Send(ICommandStream stream)
        {
            return stream.Send<byte>(
                string.Format(CommandStringPattern, _volume),
                responses => responses
                    .OfType<VolumeResponse>()
                    .Where(response => response.Volume == _volume)
                    .Select(response => response.Volume)
            );
        }
    }
}