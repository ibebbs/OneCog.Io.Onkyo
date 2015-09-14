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
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface IIscpStream : IObservable<IPacket>
    {
        IDisposable Connect();

        void Send(IPacket packet);
    }

    public class IscpStream : IIscpStream, IObserver<IPacket>
    {
        private const string Header = "ISCP";

        private readonly string _hostName;
        private readonly ushort _port;
        private readonly UnitType _unitType;

        private Subject<IPacket> _outbound;
        private IConnectableObservable<IPacket> _input;
        private IConnectableObservable<int> _output;
        private SharedDisposable _subscription;
        private ITcpClient _tcpClient;

        public IscpStream(string hostName, ushort port, UnitType unitType, ITcpClient tcpClient = null)
        {
            _hostName = hostName;
            _port = port;
            _unitType = unitType;
            _tcpClient = tcpClient ?? new TcpClient();
            _outbound = new Subject<IPacket>();

            _input = Observable.Using(ct => _tcpClient.GetDataReader(hostName, port), (reader, ct) => FromDataReader(reader)).Publish();
            _output = Observable.Using(ct => _tcpClient.GetDataWriter(hostName, port), (writer, ct) => FromDataWriter(writer)).Publish();

            _subscription = new SharedDisposable(
                () => new CompositeDisposable(
                    _input.Connect(),
                    _output.Connect()
                )
            );
        }

        private static async Task<int> WritePacket(IDataWriter dataWriter, byte[] packet)
        {
            // packet = new byte[] { 73, 83, 67, 80, 0, 0, 0, 16, 0, 0, 0, 16, 1, 0, 0, 0, 33, 49, 77, 86, 76, 81, 83, 84, 78, 13 };
            byte[] other = new byte[] { 73, 83, 67, 80, 0, 0, 0, 16, 0, 0, 0, 16, 1, 0, 0, 0, 33, 49, 80, 87, 82, 81, 83, 84, 78, 13 };
            
            byte[] local = new byte[] { 73, 83, 67, 80, 0, 0, 0, 16, 0, 0, 0, 10, 1, 0, 0, 0, 33, 49, 80, 87, 82, 81, 83, 84, 78, 26 };
            //byte local = new byte[] { 73, 83, 67, 80, 0, 0, 0, 16, 0, 0, 0, 11, 1, 0, 0, 0, 33, 49, 80, 87, 82, 81, 83, 84, 78, 26, 13 };

            await dataWriter.Write(packet);
            return packet.Length;
        }

        private Task<IObservable<int>> FromDataWriter(IDataWriter dataWriter)
        {
            return Task.FromResult(_outbound.Select(packet => packet.Data).SelectMany(value => WritePacket(dataWriter, value)));
        }

        private static async Task<IObservable<Tuple<UInt32, UInt32, byte, int>>> ReadHeader(IDataReader dataReader)
        {
            byte[] header = new byte[16];

            await dataReader.Read(header);

            string start = Encoding.UTF8.GetString(header, 0, 7);
            int offset = start.IndexOf(Header);
            UInt32 headerSize = BitConverter.BigEndian.ToUInt32(header, 4 + offset);
            UInt32 dataSize = BitConverter.BigEndian.ToUInt32(header, 8 + offset);
            byte version = header[12 + offset];

            return Observable.Return(Tuple.Create(headerSize, dataSize, version, offset));
        }

        private static async Task<IPacket> ReadData(IDataReader dataReader, Tuple<UInt32, UInt32, byte, int> header)
        {
            UInt32 remaining = Convert.ToUInt32(header.Item1 + header.Item2 + header.Item4 - 16);

            byte[] data = new byte[remaining];

            if (remaining > 0)
            {
                await dataReader.Read(data);
            }

            return new Packet(header.Item1, header.Item2, header.Item3, data.Skip(header.Item4).ToArray());
        }

        private static Task<IObservable<IPacket>> FromDataReader(IDataReader dataReader)
        {
            return Task.FromResult(Observable
                .DeferAsync(ct => ReadHeader(dataReader))
                .SelectMany(header => ReadData(dataReader, header))
                .Repeat()
           );
        }

        void IObserver<IPacket>.OnCompleted()
        {
            // Not supported
        }

        void IObserver<IPacket>.OnError(Exception error)
        {
            // Not supported
        }

        void IObserver<IPacket>.OnNext(IPacket value)
        {
            _outbound.OnNext(value);
        }

        IDisposable IObservable<IPacket>.Subscribe(IObserver<IPacket> observer)
        {
            return _input.Subscribe(observer);
        }

        public IDisposable Connect()
        {
            var disposable = _subscription.GetDisposable();

            return disposable;
        }

        public void Send(IPacket packet)
        {
            _outbound.OnNext(packet);
        }
    }
}
