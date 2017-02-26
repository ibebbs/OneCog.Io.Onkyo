using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace OneCog.Io.Onkyo.Devices
{
    public interface IReceiver : IObserver<Messages.ICommand>, IObservable<Messages.IResponse>
    {
        Task<IDisposable> Connect();
    }

    internal class Receiver : IReceiver
    {
        private readonly IscpStream _iscpStream;
        private readonly IPacketFactory _packetFactory;
        private readonly Messages.AbstractParser _parser;
        private readonly Subject<Messages.ICommand> _commands;
        private readonly IObservable<Messages.IResponse> _responses;

        public Receiver(string host, ushort port, TimeSpan commandTimeout, IEnumerable<Messages.IParser> parsers)
        {
            _iscpStream = new IscpStream(host, port, UnitType.Receiver);
            _packetFactory = PacketFactory.Default;
            _parser = new Messages.AbstractParser(parsers);

            _commands = new Subject<Messages.ICommand>();
            _responses = _iscpStream
                .Select(_packetFactory.ExtractBody)
                .SelectMany(_parser.Parse);
        }

        public async Task<IDisposable> Connect()
        {
            IDisposable commands = _commands
                    .Select(command => _packetFactory.CreatePacket(UnitType.Receiver, command.CommandString))
                    .Subscribe(_iscpStream);
            IDisposable stream = await _iscpStream.Connect();

            return new CompositeDisposable(commands, stream);
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnNext(Messages.ICommand value)
        {
            _commands.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<Messages.IResponse> observer)
        {
            return _responses.Subscribe(observer);
        }
    }
}
