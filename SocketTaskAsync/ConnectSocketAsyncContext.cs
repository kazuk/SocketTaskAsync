using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    public class ConnectSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        internal TaskCompletionSource<Socket> CompletionSource { get; set; }

        public ConnectSocketAsyncContext()
        {
            _eventArgs.Completed += CompleteAsync;
        }

        private void CompleteAsync(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                CompletionSource.SetResult(sender as Socket);
            }
            else
            {
                CompletionSource.SetException(new AsyncSocketErrorException(SocketError.SocketError));
            }
        }

        public SocketAsyncEventArgs EventArgs
        {
            get { return _eventArgs; }
        }

        internal void CompleteSynclonus(Socket socket)
        {
            CompletionSource.SetResult(socket);
        }
    }
}