using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    public class AcceptSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        public AcceptSocketAsyncContext()
        {
            _eventArgs.Completed += CompleteAsync;
        }

        private void CompleteAsync(object sender, SocketAsyncEventArgs e)
        {
            CompletionSource.SetResult(e.AcceptSocket);
        }

        internal TaskCompletionSource<Socket> CompletionSource { get; set; }

        public SocketAsyncEventArgs EventArgs
        {
            get { return _eventArgs; }
        }

        internal void CompleteSynclonus()
        {
            CompletionSource.SetResult(EventArgs.AcceptSocket);
        }
    }
}