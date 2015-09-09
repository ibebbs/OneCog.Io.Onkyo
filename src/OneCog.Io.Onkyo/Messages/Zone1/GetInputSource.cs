using OneCog.Io.Onkyo.Common;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone1
{
    internal class GetInputSource : ICommand<InputSource>
    {
        private const string CommandString = "SLIQSTN";
        
        public Task<Fallible<InputSource>> Send(ICommandStream stream)
        {
            return stream.Send<InputSource>(
                CommandString,
                responses => responses
                    .OfType<InputSourceResponse>()
                    .Select(response => response.InputSource)
            );
        }
    }
}
