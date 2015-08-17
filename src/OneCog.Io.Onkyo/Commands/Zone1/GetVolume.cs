using OneCog.Io.Onkyo.Responses.Zone1;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Commands.Zone1
{
    public class GetVolume : ICommand<byte>
    {
        private const string CommandString = "MVLQSTN";

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
