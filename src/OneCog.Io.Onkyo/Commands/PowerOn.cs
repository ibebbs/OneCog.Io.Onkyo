using OneCog.Io.Onkyo.Common;
using OneCog.Io.Onkyo.Responses;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Commands
{
    public class PowerOn : ICommand<PowerState>
    {
        private const string CommandString = "PWR01";
        
        public Task<Fallible<PowerState>> Send(ICommandStream stream)
        {
            return stream.Send<PowerState>(CommandString, 
                response => response
                    .OfType<PowerStateResponse>()
                    .Where(r => r.PowerState == PowerState.On)
                    .Select(r => r.PowerState)
            );
        }
    }
}
