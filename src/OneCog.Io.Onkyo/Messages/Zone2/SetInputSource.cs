using OneCog.Io.Onkyo.Common;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    internal class SetInputSource : ICommand<InputSource>
    {
        private const string CommandStringPattern = "SLZ{0:X2}";
        
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
