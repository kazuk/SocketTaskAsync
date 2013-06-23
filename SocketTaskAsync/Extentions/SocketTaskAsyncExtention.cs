using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync.Extentions
{
    public static class SocketTaskAsyncExtention
    {
        public static Task<Socket> AcceptTaskAsync(this Socket socket)
        {
            var completionSource = new TaskCompletionSource<Socket>();
            var asyncArgs = new SocketAsyncEventArgs();
            EventHandler<SocketAsyncEventArgs> asyncArgsOnCompleted = (sender, args) =>
                {
                    if (args.SocketError != SocketError.Success)
                    {
                        completionSource.SetException( new AsyncSocketErrorException( args.SocketError) );
                        return;
                    }
                    completionSource.SetResult(args.AcceptSocket);
                };
            asyncArgs.Completed += asyncArgsOnCompleted;

            if (!socket.AcceptAsync(asyncArgs))
            {
                asyncArgsOnCompleted(socket, asyncArgs);
            }
            
            return completionSource.Task;
        }

        public static Task<Socket> AcceptTaskAsync(this Socket socket, AcceptSocketAsyncContext context)
        {
            var completionSource = new TaskCompletionSource<Socket>();
            context.CompletionSource = completionSource;
            if (!socket.AcceptAsync(context.EventArgs))
            {
                context.CompleteSynclonus();
            }
            return completionSource.Task;
        }

        /// <summary>
        /// Socket.Connect(EndPoint) �ɑ΂��� Task Async �����񋟂��܂�
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remoteEp"></param>
        /// <returns></returns>
        /// <remarks>Socket.Connect��void��TaskCompletionSource���g���̂ɍ������̂�Socket��Ԃ��悤�ɂ��܂���</remarks>
        public static Task<Socket> ConnectTaskAsync(this Socket socket,EndPoint remoteEp)
        {
            var asyncArgs = new SocketAsyncEventArgs {RemoteEndPoint = remoteEp};
            var completionSource = new TaskCompletionSource<Socket>();
            EventHandler<SocketAsyncEventArgs> asyncArgsOnCompleted = (sender, e) =>
                {
                    if (e.SocketError != SocketError.Success)
                    {
                        completionSource.SetException(new AsyncSocketErrorException(SocketError.SocketError));
                        return;
                    }
                    completionSource.SetResult(sender as Socket);
                };
            asyncArgs.Completed += asyncArgsOnCompleted;
            if (! socket.ConnectAsync(asyncArgs))
            {
                asyncArgsOnCompleted(socket, asyncArgs);
            }
            return completionSource.Task;
        }

        public static Task<Socket> ConnectTaskAsync(this Socket socket, ConnectSocketAsyncContext context,
                                                    EndPoint remoteEp)
        {
            var completionSource = new TaskCompletionSource<Socket>();
            context.CompletionSource = completionSource;
            var socketAsyncEventArgs = context.EventArgs;
            socketAsyncEventArgs.RemoteEndPoint = remoteEp;
            if (!socket.ConnectAsync(socketAsyncEventArgs))
            {
                context.CompleteSynclonus(socket);
            }
            return completionSource.Task;
        }
    }
}
