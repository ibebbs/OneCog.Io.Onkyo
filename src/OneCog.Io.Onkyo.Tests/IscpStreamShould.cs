using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        private static readonly byte[] PaddedVersion = new byte[] { Version, 0, 0, 0 };

        [Test]
        public async Task ConnectToTcpClientWhenConnectIsCalled()
        {
            ITcpClient tcpClient = A.Fake<ITcpClient>();

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            await subject.Connect();

            A.CallTo(() => tcpClient.ConnectAsync("localhost", 60128)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private Task<int> FromStream(byte[] buffer, Stream stream)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            if (stream.Position < stream.Length)
            {
                stream.Read(buffer, 0, buffer.Length);
                tcs.SetResult(buffer.Length);
            }

            return tcs.Task;
        }

        private IEnumerable<byte> CreateIscpPacket(string command)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(command).Concat(new[] { Eof }).ToArray();
            byte[] header = new byte[16];
            Array.Copy(ASCIIEncoding.ASCII.GetBytes("ISCP"), 0, header, 0, 4);
            Array.Copy(Onkyo.BitConverter.BigEndian.GetBytes((UInt32)16), 0, header, 4, 4);
            Array.Copy(Onkyo.BitConverter.BigEndian.GetBytes((UInt32)data.Length), 0, header, 8, 4);
            Array.Copy(PaddedVersion, 0, header, 12, 4);

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
        public async Task CorrectlyParseAReceivedPacket()
        {
            Stream source = CreateIscpStream("!1PWR01");

            INetworkStream networkStream = A.Fake<INetworkStream>();
            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetStream()).Returns(networkStream);
            A.CallTo(() => networkStream.ReadAsync(A<byte[]>.Ignored, 0, A<int>.Ignored, A<CancellationToken>.Ignored)).ReturnsLazily(call => FromStream(call.GetArgument<byte[]>(0), source));
            A.CallTo(() => networkStream.WriteAsync(A<byte[]>.Ignored, 0, A<int>.Ignored, A<CancellationToken>.Ignored)).Returns(new TaskCompletionSource<object>().Task);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            List<IPacket> packets = new List<IPacket>();
            subject.Received.Subscribe(packets.Add);

            await subject.Connect();

            Assert.That(packets.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CorrectlyParseMultipleReceivedPackets()
        {
            Stream source = CreateIscpStream(new [] { "!1PWR01", "!1PWR02", "!1PWR03" });

            INetworkStream networkStream = A.Fake<INetworkStream>();
            ITcpClient tcpClient = A.Fake<ITcpClient>();
            A.CallTo(() => tcpClient.GetStream()).Returns(networkStream);
            A.CallTo(() => networkStream.ReadAsync(A<byte[]>.Ignored, 0, A<int>.Ignored, A<CancellationToken>.Ignored)).ReturnsLazily(call => FromStream(call.GetArgument<byte[]>(0), source));
            A.CallTo(() => networkStream.WriteAsync(A<byte[]>.Ignored, 0, A<int>.Ignored, A<CancellationToken>.Ignored)).Returns(new TaskCompletionSource<object>().Task);

            IscpStream subject = new IscpStream("localhost", 60128, UnitType.Receiver, tcpClient);

            List<IPacket> packets = new List<IPacket>();
            subject.Received.Subscribe(packets.Add);

            await subject.Connect();

            Assert.That(packets.Count, Is.EqualTo(3));
        }
    }
}
