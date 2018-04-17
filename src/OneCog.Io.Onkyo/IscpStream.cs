using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            _tcpClient = new ManagedTcpClient(tcpClient);

            _outbound = new Subject<IPacket>();
            _inbound = new Subject<IPacket>();
        }

        private async Task<int> WritePacket(INetworkStream stream, byte[] packet, CancellationToken cancellationToken)
        {
            await stream.WriteAsync(packet, 0, packet.Length, cancellationToken);
            return packet.Length;
        }

        private async Task<IObservable<Tuple<UInt32, UInt32, byte, int>>> ReadHeader(INetworkStream stream, CancellationToken cancellationToken)
        {
            byte[] header = new byte[16];

            await stream.ReadAsync(header, 0, header.Length, cancellationToken);

            string start = Encoding.UTF8.GetString(header, 0, 7);
            int offset = start.IndexOf(Header);
            UInt32 headerSize = BitConverter.BigEndian.ToUInt32(header, 4 + offset);
            UInt32 dataSize = BitConverter.BigEndian.ToUInt32(header, 8 + offset);
            byte version = header[12 + offset];

            return Observable.Return(Tuple.Create(headerSize, dataSize, version, offset));
        }

        private async Task<IPacket> ReadData(INetworkStream stream, Tuple<UInt32, UInt32, byte, int> header, CancellationToken cancellationToken)
        {
            UInt32 remaining = Convert.ToUInt32(header.Item1 + header.Item2 + header.Item4 - 16);

            byte[] data = new byte[remaining];

            if (remaining > 0)
            {
                await stream.ReadAsync(data, 0, data.Length, cancellationToken);
            }

            return new Packet(header.Item1, header.Item2, header.Item3, data.Skip(header.Item4).ToArray());
        }

        public async Task<IDisposable> Connect()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            
            await _tcpClient.ConnectAsync(_hostName, _port);

            INetworkStream stream = _tcpClient.GetStream();

            IDisposable outbound = _outbound
                .Select(packet => packet.Data)
                .SelectMany(value => WritePacket(stream, value, cts.Token))
                .Subscribe();

            IDisposable inbound = Observable
                .DeferAsync(ct => ReadHeader(stream, cts.Token))
                .SelectMany(header => ReadData(stream, header, cts.Token))
                .Repeat()
                .Subscribe(_inbound);

            return new CompositeDisposable(
                new CancellationDisposable(cts),
                outbound,
                inbound,
                stream,
                _tcpClient,
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
