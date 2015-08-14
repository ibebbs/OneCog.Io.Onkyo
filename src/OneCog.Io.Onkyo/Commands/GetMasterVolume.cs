using OneCog.Io.Onkyo.Responses;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Commands
{
    public class GetMasterVolume : ICommand<byte>
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
