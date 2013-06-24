using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Send/SendTo に関わる SocketAsyncEventArgs を保持します。
    /// </summary>
    public class SendSokectAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        /// <summary>
        /// 関連付けられた Task Async の完了ソースを保持します
        /// </summary>
        public TaskCompletionSource<int> CompletionSource { get; set; }

        /// <summary>
        /// 保持されている SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs EventArgs
        {
            get { return _eventArgs; }
        }

        internal void CompletedSynclonus()
        {
            if (_eventArgs.SocketError != SocketError.Success)
            {
                CompletionSource.SetException(new AsyncSocketErrorException(_eventArgs.SocketError));
                return;
            }
            CompletionSource.SetResult(_eventArgs.BytesTransferred);
        }
    }
}