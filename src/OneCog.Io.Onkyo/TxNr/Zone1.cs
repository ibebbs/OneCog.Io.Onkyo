using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.TxNr
{
    public interface IZone1 : IZone
    {
    }

    internal class Zone1 : IZone1
    {
        public Zone1()
        {

        }

        public Task TurnOn()
        {
            throw new NotImplementedException();
        }

        public Task TurnOff()
        {
            throw new NotImplementedException();
        }

        public Task<PowerState> GetPowerState()
        {
            throw new NotImplementedException();
        }
    }
}
