using OneCog.Io.Onkyo.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Devices
{
    public interface IReceiver
    {
        IDisposable Connect();

        Task<Fallible<T>> Send<T>(ICommand<T> command);
    }

    internal class Receiver : IReceiver
    {
        private readonly ICommandStream _commandStream;

        public Receiver(string host, ushort port, IEnumerable<Messages.IParser> parsers)
        {
            _commandStream = new CommandStream(new IscpStream(host, port, UnitType.Receiver), PacketFactory.Default, new AbstractParser(parsers), UnitType.Receiver, TimeSpan.FromMilliseconds(500));
        }

        public Receiver(string host, ushort port, ICommandStream commandStream)
        {
            _commandStream = commandStream;
        }

        public IDisposable Connect()
        {
            return _commandStream.Connect();
        }

        public Task<Fallible<T>> Send<T>(ICommand<T> command)
        {
            return command.Send(_commandStream);
        }
    }
}
