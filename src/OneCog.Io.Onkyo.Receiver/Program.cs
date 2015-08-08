using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OneCog.Io.Onkyo.Receiver
{
    class Program
    {
        private static TcpListener _socket;

        static void Main(string[] args)
        {
            _socket = new TcpListener(IPAddress.Loopback, 60128);
            _socket.Start();

            Observable
                .FromAsync(_socket.AcceptTcpClientAsync)
                .Select(tcpClient => tcpClient.GetStream())
                .Select(stream => new StreamReader(stream))
                .Select(reader => Observable.FromAsync(reader.ReadLineAsync).Repeat())
                .SelectMany(observable => observable)
                .Subscribe(sent => Console.WriteLine(sent));

            Console.WriteLine("Receiver listening...");
            Console.ReadLine();
        }
    }
}
