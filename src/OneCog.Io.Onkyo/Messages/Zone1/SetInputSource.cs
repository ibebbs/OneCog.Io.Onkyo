using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone1
{
    internal class SetInputSource : ICommand<InputSource>
    {
        private const string CommandStringPattern = "SLI{0:X2}";
        
        private InputSource _inputSource;

        public SetInputSource(InputSource inputSource)
        {
            _inputSource = inputSource;
        }

        public Task<Fallible<InputSource>> Send(ICommandStream stream)
        {
            return stream.Send<InputSource>(
                string.Format(CommandStringPattern, _inputSource.Id),
                responses => responses
                    .OfType<InputSourceResponse>()
                    .Where(response => response.InputSource == _inputSource)
                    .Select(response => response.InputSource)
            );
        }
    }
}
