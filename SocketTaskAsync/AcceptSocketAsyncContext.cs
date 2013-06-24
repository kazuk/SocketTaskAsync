using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Accept に利用される SocketAsyncEventArgs を保持します。
    /// </summary>
    public class AcceptSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        /// <summary>
        /// Accept用 SocketAsyncEventArgs を構築します。
        /// </summary>
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

        /// <summary>
        /// 保持される SocketAsyncEventArgs を取得します。
        /// </summary>
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