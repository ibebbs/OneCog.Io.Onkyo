using System;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface ITcpClient : IDisposable
    {
        Task ConnectAsync(string host, int port);
        INetworkStream GetStream();
    }
}
