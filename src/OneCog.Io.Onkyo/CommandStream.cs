using OneCog.Io.Onkyo.Commands;
using OneCog.Io.Onkyo.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface ICommandStream
    {
        Task<Fallible<T>> Send<T>(string command, Func<IObservable<IResponse>, IObservable<T>> responseProjection);
    }
     
    internal class CommandStream : ICommandStream
    {
        private readonly IIscpStream _stream;
        private readonly IPacketFactory _packetFactory;
        private readonly UnitType _unitType;
        private readonly IObservable<IResponse> _responses;
        private readonly TimeSpan _commandTimeout;

        public CommandStream(IIscpStream stream, IPacketFactory packetFactory, IAbstractParser parser, UnitType unitType, TimeSpan commandTimeout)
        {
            _stream = stream;
            _packetFactory = packetFactory;
            _unitType = unitType;
            _commandTimeout = commandTimeout;

            _responses = _stream
                .Select(_packetFactory.ExtractBody)
                .SelectMany(parser.Parse)
                .Publish().RefCount();
        }

        public Task<Fallible<T>> Send<T>(string command, Func<IObservable<IResponse>, IObservable<T>> responseProjection)
        {
            Task<Fallible<T>> task = responseProjection(_responses)
                .Select(result => Fallible.Success(result))
                .Timeout(_commandTimeout)
                .Catch<Fallible<T>, Exception>(exception => Observable.Return(Fallible.Fail<T>(exception)))
                .Take(1)
                .ToTask();

            IPacket packet = _packetFactory.CreatePacket(_unitType, command);

            _stream.Send(packet);

            return task;
        }
    }
}
