using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public class NetworkStreamDecorator : INetworkStream
    {
        private readonly NetworkStream _networkStream;

        public NetworkStreamDecorator(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public void Dispose()
        {
            _networkStream.Dispose();
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            return _networkStream.ReadAsync(buffer, offset, size, cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            return _networkStream.WriteAsync(buffer, offset, size, cancellationToken);
        }
    }
}
