using OneCog.Io.Onkyo.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Devices
{
    public interface IReceiver
    {
        Task<IDisposable> Connect();

        Task<Fallible<T>> Send<T>(ICommand<T> command);
    }

    internal class Receiver : IReceiver
    {
        private readonly ICommandStream _commandStream;

        public Receiver(string host, ushort port, TimeSpan commandTimeout, IEnumerable<Messages.IParser> parsers)
        {
            _commandStream = new CommandStream(new IscpStream(host, port, UnitType.Receiver), PacketFactory.Default, new AbstractParser(parsers), UnitType.Receiver, commandTimeout);
        }

        public Receiver(string host, ushort port, ICommandStream commandStream)
        {
            _commandStream = commandStream;
        }

        public Task<IDisposable> Connect()
        {
            return _commandStream.Connect();
        }

        public Task<Fallible<T>> Send<T>(ICommand<T> command)
        {
            return command.Send(_commandStream);
        }
    }
}
