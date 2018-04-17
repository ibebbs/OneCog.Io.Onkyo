using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface INetworkStream : IDisposable
    {
        Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken);
        Task WriteAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken);
    }
}
