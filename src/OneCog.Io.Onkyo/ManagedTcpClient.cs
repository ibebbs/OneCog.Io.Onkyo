using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public class ManagedTcpClient : ITcpClient
    {
        private readonly bool _shouldDispose;
        private ITcpClient _tcpClient;

        public ManagedTcpClient() : this(null) { }

        public ManagedTcpClient(ITcpClient tcpClient)
        {
            if (tcpClient != null)
            {
                _tcpClient = tcpClient;
                _shouldDispose = false;
            }
            else
            {
                _tcpClient = new TcpClientDecorator();
                _shouldDispose = true;
            }
        }

        public void Dispose()
        {
            if (_shouldDispose && _tcpClient != null)
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
            return _tcpClient.GetStream();
        }
    }
}
