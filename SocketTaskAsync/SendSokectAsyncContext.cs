using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    public class SendSokectAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        public TaskCompletionSource<int> CompletionSource { get; set; }

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