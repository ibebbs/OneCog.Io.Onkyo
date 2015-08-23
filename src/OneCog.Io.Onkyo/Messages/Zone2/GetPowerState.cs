using OneCog.Io.Onkyo.Common;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    public class GetPowerState : ICommand<PowerState>
    {
        private const string CommandString = "ZPWQSTN";

        public Task<Fallible<PowerState>> Send(ICommandStream stream)
        {
            return stream.Send<PowerState>(
                CommandString,
                responses => responses
                    .OfType<PowerStateResponse>()
                    .Select(response => response.PowerState)
            );
        }
    }
}
