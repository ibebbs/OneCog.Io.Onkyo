using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.TxNr
{
    public class Receiver
    {
        public Receiver()
        {
            Zone1 = new Zone1();
            Zone2 = new Zone2();
            Zone3 = new Zone3();
        }

        public IZone1 Zone1 { get; private set; }

        public IZone2 Zone2 { get; private set; }

        public IZone3 Zone3 { get; private set; }
    }
}
