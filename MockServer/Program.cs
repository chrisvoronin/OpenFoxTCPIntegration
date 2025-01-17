using System.Threading;

namespace MockServer
{
    internal class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static TcpServer server = new TcpServer();

        static void Main(string[] args)
        {
            server.Work();
            _quitEvent.WaitOne();
        }
    }
}
