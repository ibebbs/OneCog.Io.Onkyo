using FakeItEasy;
using NUnit.Framework;
using OneCog.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Tests
{
    [TestFixture]
    public class IscpStreamShould
    {
        private static readonly byte[] IscpHeader = ASCIIEncoding.ASCII.GetBytes("ISCP");
        private static readonly byte Eof = 16;
        private static readonly byte Version = 1;
        private static readonly byte[] Padding = new byte[3];
        private static readonly byte[] VersionPadding = new byte[] { Version, 0, 0, 0 };

        [Test]
        public void ConnectToTcpClientWhenConnectIsCalled()
        {
            ITcpClient tcpClient = A.Fake<ITcpClient>();

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            subject.Connect();

            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void OnlyConnectToTcpClientOnce()
        {
            ITcpClient tcpClient = A.Fake<ITcpClient>();

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            subject.Connect();
            subject.Connect();
            subject.Connect();

            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void DisconnectTcpClientWhenConnectionIsDisposed()
        {
            IDataReader dataReader = A.Fake<IDataReader>();
            A.CallTo(() => dataReader.Read(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);
            IDataWriter dataWriter = A.Fake<IDataWriter>();
            A.CallTo(() => dataWriter.Write(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);
            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).Returns(dataReader);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).Returns(dataWriter);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            IDisposable connection = subject.Connect();

            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => dataReader.Dispose()).MustNotHaveHappened();
            A.CallTo(() => dataWriter.Dispose()).MustNotHaveHappened();

            connection.Dispose();

            A.CallTo(() => dataReader.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => dataWriter.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void DisconnectTcpClientWhenLastConnectionIsDisposed()
        {
            IDataReader dataReader = A.Fake<IDataReader>();
            A.CallTo(() => dataReader.Read(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);
            IDataWriter dataWriter = A.Fake<IDataWriter>();
            A.CallTo(() => dataWriter.Write(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);

            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).Returns(dataReader);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).Returns(dataWriter);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            IDisposable connection1 = subject.Connect();
            IDisposable connection2 = subject.Connect();

            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);

            connection1.Dispose();

            A.CallTo(() => dataReader.Dispose()).MustNotHaveHappened();
            A.CallTo(() => dataWriter.Dispose()).MustNotHaveHappened();

            connection2.Dispose();

            A.CallTo(() => dataReader.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => dataWriter.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        private Task FromStream(byte[] buffer, Stream stream)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (stream.Position < stream.Length)
            {
                stream.Read(buffer, 0, buffer.Length);
                tcs.SetResult(null);
            }

            return tcs.Task;
        }

        private IEnumerable<byte> CreateIscpPacket(string command)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(command).Concat(new[] { Eof }).ToArray();
            byte[] header = new byte[16];
            Array.Copy(ASCIIEncoding.ASCII.GetBytes("ISCP"), 0, header, 0, 4);
            Array.Copy(BitConverter.BigEndian.GetBytes((UInt32)16), 0, header, 4, 4);
            Array.Copy(BitConverter.BigEndian.GetBytes((UInt32)data.Length + 16), 0, header, 8, 4);
            Array.Copy(VersionPadding, 0, header, 4, 4);

            return header.Concat(data);
        }

        private Stream CreateIscpStream(string command)
        {
            return CreateIscpPacket(command).AsStream();
        }

        private Stream CreateIscpStream(IEnumerable<string> commands)
        {
            return commands.SelectMany(CreateIscpPacket).AsStream();
        }

        [Test]
        public void CorrectlyParseAReceivedPacket()
        {
            Stream source = CreateIscpStream("!1PWR01");

            IDataReader dataReader = A.Fake<IDataReader>();
            A.CallTo(() => dataReader.Read(A<byte[]>.Ignored)).ReturnsLazily(call => FromStream(call.GetArgument<byte[]>(0), source));
            IDataWriter dataWriter = A.Fake<IDataWriter>();
            A.CallTo(() => dataWriter.Write(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);

            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).Returns(dataReader);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).Returns(dataWriter);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            List<IPacket> packets = new List<IPacket>();
            subject.Subscribe(packets.Add);

            subject.Connect();

            Assert.That(packets.Count, Is.EqualTo(1));
        }

        [Test]
        public void CorrectlyParseMultipleReceivedPackets()
        {
            Stream source = CreateIscpStream(new [] { "!1PWR01", "!1PWR02", "!1PWR03" });

            IDataReader dataReader = A.Fake<IDataReader>();
            A.CallTo(() => dataReader.Read(A<byte[]>.Ignored)).ReturnsLazily(call => FromStream(call.GetArgument<byte[]>(0), source));
            IDataWriter dataWriter = A.Fake<IDataWriter>();
            A.CallTo(() => dataWriter.Write(A<byte[]>.Ignored)).Returns(new TaskCompletionSource<object>().Task);

            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetDataReader("localhost", 60128)).Returns(dataReader);
            A.CallTo(() => tcpClient.GetDataWriter("localhost", 60128)).Returns(dataWriter);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            List<IPacket> packets = new List<IPacket>();
            subject.Subscribe(packets.Add);

            subject.Connect();

            Assert.That(packets.Count, Is.EqualTo(3));
        }
    }
}
