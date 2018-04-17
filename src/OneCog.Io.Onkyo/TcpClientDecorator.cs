using System.Net.Sockets;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public class TcpClientDecorator : ITcpClient
    {
        private TcpClient _tcpClient;

        public TcpClientDecorator()
        {
            _tcpClient = new TcpClient();
        }

        public void Dispose()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Dispose();
                _tcpClient = null;
            }
        }

        public Task ConnectAsync(string host, int port)
        {
            return _tcpClient.ConnectAsync(host, port);
        }

        public INetworkStream GetStream()
        {
            return new NetworkStreamDecorator(_tcpClient.GetStream());
        }
    }
}
