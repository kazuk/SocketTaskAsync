using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Connect に利用される SocketAsyncEventArgs を保持します
    /// </summary>
    public class ConnectSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        internal TaskCompletionSource<Socket> CompletionSource { get; set; }

        /// <summary>
        /// 接続用 SocketAsyncEventArgs を構築します。
        /// </summary>
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

        /// <summary>
        /// 保持される SocketAsyncEventArgs を取得します。
        /// </summary>
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