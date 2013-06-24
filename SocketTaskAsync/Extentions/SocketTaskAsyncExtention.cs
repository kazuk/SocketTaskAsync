using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync.Extentions
{
    public static class SocketTaskAsyncExtention
    {
        /// <summary>
        /// Socket.Accept Ç…ëŒÇ∑ÇÈ Task Async ëÄçÏÇíÒãüÇµÇ‹Ç∑
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static Task<Socket> AcceptTaskAsync(this Socket socket)
        {
            Contract.Requires(socket != null);

            var completionSource = new TaskCompletionSource<Socket>();
            var asyncArgs = new SocketAsyncEventArgs();
            var asyncArgsOnCompleted = CreateHandler(completionSource,e => e.AcceptSocket);
            asyncArgs.Completed += asyncArgsOnCompleted;

            if (!socket.AcceptAsync(asyncArgs))
            {
                asyncArgsOnCompleted(socket, asyncArgs);
            }
            
            return completionSource.Task;
        }

        /// <summary>
        /// Socket.Accept Ç…ëŒÇ∑ÇÈ Task Async ëÄçÏÇíÒãüÇµÇ‹Ç∑
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Task<Socket> AcceptTaskAsync(this Socket socket, AcceptSocketAsyncContext context)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(context!=null);

            var completionSource = new TaskCompletionSource<Socket>();
            context.CompletionSource = completionSource;
            if (!socket.AcceptAsync(context.EventArgs))
            {
                context.CompleteSynclonus();
            }
            return completionSource.Task;
        }

        /// <summary>
        /// Socket.Connect(EndPoint) Ç…ëŒÇ∑ÇÈ Task Async ëÄçÏÇíÒãüÇµÇ‹Ç∑
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remoteEp"></param>
        /// <returns></returns>
        /// <remarks>Socket.ConnectÇÕvoidÇ≈TaskCompletionSourceÇégÇ§ÇÃÇ…ç¢Ç¡ÇΩÇÃÇ≈SocketÇï‘Ç∑ÇÊÇ§Ç…ÇµÇ‹ÇµÇΩ</remarks>
        public static Task<Socket> ConnectTaskAsync(this Socket socket,EndPoint remoteEp)
        {
            Contract.Requires(socket!=null);

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
            Contract.Requires(socket!=null);
            Contract.Requires(context!=null);

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

        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer, int size)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(buffer.Length>=size);

            var taskCompletionSource = new TaskCompletionSource<int>();
            var socketAsyncArgs = new SocketAsyncEventArgs();
            var socketAsyncArgsOnCompleted = CreateHandler(taskCompletionSource,e => e.BytesTransferred);
            socketAsyncArgs.Completed += socketAsyncArgsOnCompleted;
            socketAsyncArgs.SetBuffer(buffer, 0, size);

            if (!socket.SendAsync(socketAsyncArgs))
            {
                socketAsyncArgsOnCompleted(socket, socketAsyncArgs);
            }
            return taskCompletionSource.Task;
        }

        public static Task<int> SendTaskAsync(this Socket socket, SendSokectAsyncContext context,
                                              byte[] buffer, int size)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(context!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(buffer.Length>=size);

            var completionSource = new TaskCompletionSource<int>();
            context.CompletionSource = completionSource;
            SocketAsyncEventArgs args = context.EventArgs;
            args.SetBuffer(buffer,0,size);
            if (!socket.SendAsync(args))
            {
                context.CompletedSynclonus();
            }
            return completionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="socketFlags"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public static Task<int> SendToTaskAsync(this Socket socket, byte[] buffer, int offset, int count, SocketFlags socketFlags, EndPoint remoteEP)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(buffer.Length>=offset);
            Contract.Requires(buffer.Length>= offset+count);

            var taskCompletionSource = new TaskCompletionSource<int>();
            var socketAsyncArgs = new SocketAsyncEventArgs {RemoteEndPoint = remoteEP, SocketFlags = socketFlags};
            socketAsyncArgs.SetBuffer(buffer,offset,count);
            var socketAsyncArgsOnCompleted = CreateHandler(taskCompletionSource, e => e.BytesTransferred);
            socketAsyncArgs.Completed += socketAsyncArgsOnCompleted;
            if (!socket.SendToAsync(socketAsyncArgs))
            {
                socketAsyncArgsOnCompleted(socket, socketAsyncArgs);
            }
            return taskCompletionSource.Task;
        }

        private static EventHandler<SocketAsyncEventArgs> CreateHandler<T>(TaskCompletionSource<T> taskCompletionSource, Func<SocketAsyncEventArgs, T> resultSelector)
        {
            return (sender, args) =>
                {
                    if (args.SocketError != SocketError.Success)
                    {
                        taskCompletionSource.SetException(new AsyncSocketErrorException(args.SocketError));
                        return;
                    }
                    taskCompletionSource.SetResult(resultSelector(args));
                };
        }
    }
}
