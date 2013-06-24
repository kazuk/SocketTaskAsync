using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Accept Ç…óòópÇ≥ÇÍÇÈ SocketAsyncEventArgs Çï€éùÇµÇ‹Ç∑ÅB
    /// </summary>
    public class AcceptSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        public AcceptSocketAsyncContext()
        {
            _eventArgs.Completed += CompleteAsync;
        }

        private void CompleteAsync(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                CompletionSource.SetResult(e.AcceptSocket);
            }
            else
            {
                CompletionSource.SetException( new AsyncSocketErrorException(SocketError.SocketError));
            }
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