using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Accept �ɗ��p����� SocketAsyncEventArgs ��ێ����܂��B
    /// </summary>
    public class AcceptSocketAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        /// <summary>
        /// Accept�p SocketAsyncEventArgs ���\�z���܂��B
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
        /// �ێ������ SocketAsyncEventArgs ���擾���܂��B
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