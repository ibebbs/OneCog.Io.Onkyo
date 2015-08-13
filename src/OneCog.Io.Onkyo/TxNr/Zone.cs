using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.TxNr
{
    public interface IZone
    {
        Task TurnOn();

        Task TurnOff();

        Task<PowerState> GetPowerState();
    }
}
