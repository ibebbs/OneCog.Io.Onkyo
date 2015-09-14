using OneCog.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface IIscpStream
    {
        Task<IDisposable> Connect();

        void Send(IPacket packet);

        IObservable<IPacket> Received { get; }
    }

    public class IscpStream : IIscpStream
    {
        private const string Header = "ISCP";

        private readonly string _hostName;
        private readonly ushort _port;
        private readonly UnitType _unitType;
        private readonly ITcpClient _tcpClient;

        private Subject<IPacket> _outbound;
        private Subject<IPacket> _inbound;

        public IscpStream(string hostName, ushort port, UnitType unitType, ITcpClient tcpClient = null)
        {
            _hostName = hostName;
            _port = port;
            _unitType = unitType;
            _tcpClient = tcpClient ?? new TcpClient();

            _outbound = new Subject<IPacket>();
            _inbound = new Subject<IPacket>();
        }

        private async Task<int> WritePacket(byte[] packet, CancellationToken cancellationToken)
        {
            await _tcpClient.Write(packet, cancellationToken);
            return packet.Length;
        }

        private async Task<IObservable<Tuple<UInt32, UInt32, byte, int>>> ReadHeader(CancellationToken cancellationToken)
        {
            byte[] header = new byte[16];

            await _tcpClient.Read(header, cancellationToken);

            string start = Encoding.UTF8.GetString(header, 0, 7);
            int offset = start.IndexOf(Header);
            UInt32 headerSize = BitConverter.BigEndian.ToUInt32(header, 4 + offset);
            UInt32 dataSize = BitConverter.BigEndian.ToUInt32(header, 8 + offset);
            byte version = header[12 + offset];

            return Observable.Return(Tuple.Create(headerSize, dataSize, version, offset));
        }

        private async Task<IPacket> ReadData(Tuple<UInt32, UInt32, byte, int> header, CancellationToken cancellationToken)
        {
            UInt32 remaining = Convert.ToUInt32(header.Item1 + header.Item2 + header.Item4 - 16);

            byte[] data = new byte[remaining];

            if (remaining > 0)
            {
                await _tcpClient.Read(data, cancellationToken);
            }

            return new Packet(header.Item1, header.Item2, header.Item3, data.Skip(header.Item4).ToArray());
        }

        public async Task<IDisposable> Connect()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            IDisposable connection = await _tcpClient.Connect(_hostName, _port, cts.Token);

            IDisposable outbound = _outbound
                .Select(packet => packet.Data)
                .SelectMany(value => WritePacket(value, cts.Token))
                .Subscribe();

            IDisposable inbound = Observable
                .DeferAsync(ct => ReadHeader(cts.Token))
                .SelectMany(header => ReadData(header, cts.Token))
                .Repeat()
                .Subscribe(_inbound);

            return new CompositeDisposable(
                new CancellationDisposable(cts),
                outbound,
                inbound,
                connection,
                cts
            );
        }

        public void Send(IPacket packet)
        {
            _outbound.OnNext(packet);
        }

        public IObservable<IPacket> Received
        {
            get { return _inbound; }
        }
    }
}
