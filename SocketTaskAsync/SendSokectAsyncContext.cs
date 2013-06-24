using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync
{
    /// <summary>
    /// Send/SendTo �Ɋւ�� SocketAsyncEventArgs ��ێ����܂��B
    /// </summary>
    public class SendSokectAsyncContext
    {
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        /// <summary>
        /// �֘A�t����ꂽ Task Async �̊����\�[�X��ێ����܂�
        /// </summary>
        public TaskCompletionSource<int> CompletionSource { get; set; }

        /// <summary>
        /// �ێ�����Ă��� SocketAsyncEventArgs
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