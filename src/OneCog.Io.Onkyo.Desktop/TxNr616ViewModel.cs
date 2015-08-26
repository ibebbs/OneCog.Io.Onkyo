using Caliburn.Micro;
using OneCog.Io.Onkyo.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Desktop
{
    public class TxNr616ViewModel : Screen
    {
        private IReceiver _studyAmp;
        private IDisposable _connection;
        private byte _volume;

        public TxNr616ViewModel()
        {
            _studyAmp = Tx.Nr616("192.168.1.54", 60128, TimeSpan.FromSeconds(5));
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _connection = _studyAmp.Connect();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public async Task TurnOn()
        {
            var result = await _studyAmp.Send(new Messages.Zone1.PowerOn());

            Debug.WriteLine(result);
        }

        public async Task GetVolume()
        {
            var volume = await _studyAmp.Send<byte>(new Messages.Zone1.GetVolume());

            if (volume.Succeeded)
            {
                Volume = volume.Value;
            }
        }

        public override string DisplayName
        {
            get { return "Tx-Nr616"; }
            set { }
        }

        public byte Volume
        {
            get { return _volume; }
            private set
            {
                if (value != _volume)
                {
                    _volume = value;

                    NotifyOfPropertyChange(() => Volume);
                }
            }
        }
    }
}
