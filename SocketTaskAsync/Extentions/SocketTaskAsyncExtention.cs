using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTaskAsync.Extentions
{
    /// <summary>
    /// Task Async でソケット操作を行います
    /// </summary>
    public static class SocketTaskAsyncExtention
    {
        /// <summary>
        /// Socket.Accept に対する Task Async 操作を提供します
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
        /// Socket.Accept に対する Task Async 操作を提供します
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
        /// Socket.Connect(EndPoint) に対する Task Async 操作を提供します
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remoteEp"></param>
        /// <returns></returns>
        /// <remarks>Socket.ConnectはvoidでTaskCompletionSourceを使うのに困ったのでSocketを返すようにしました</remarks>
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

        /// <summary>
        /// ConnectSocketAsyncContext を利用して remoteEp への接続を確立します。
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="remoteEp"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Socket.Send( buffer, size ) をTask Async します。
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer,int offset, int count,SocketFlags socketFlags)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(buffer.Length>=count);

            var taskCompletionSource = new TaskCompletionSource<int>();
            var socketAsyncArgs = new SocketAsyncEventArgs {SocketFlags = socketFlags};
            var socketAsyncArgsOnCompleted = CreateHandler(taskCompletionSource,e => e.BytesTransferred);
            socketAsyncArgs.Completed += socketAsyncArgsOnCompleted;
            socketAsyncArgs.SetBuffer(buffer, offset, count);

            if (!socket.SendAsync(socketAsyncArgs))
            {
                socketAsyncArgsOnCompleted(socket, socketAsyncArgs);
            }
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer, int offset, int count)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(offset>=0 );
            Contract.Requires(count>=0);
            Contract.Requires(buffer.Length>=offset);
            Contract.Requires(buffer.Length>=offset+count);

            return SendTaskAsync(socket, buffer, offset, count, SocketFlags.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket,SendSokectAsyncContext context,  byte[] buffer, int offset, int count)
        {
            Contract.Requires(socket != null);
            Contract.Requires(context!=null);
            Contract.Requires(buffer != null);
            Contract.Requires(offset >= 0);
            Contract.Requires(count >= 0);
            Contract.Requires(buffer.Length >= offset);
            Contract.Requires(buffer.Length >= offset + count);

            return SendTaskAsync(socket,context, buffer, offset, count, SocketFlags.None);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer, int size,SocketFlags socketFlags)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(size>=0);
            Contract.Requires(buffer.Length>=size);

            return SendTaskAsync(socket, buffer, 0, size, socketFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, SendSokectAsyncContext context, byte[] buffer, int size, SocketFlags socketFlags)
        {
            Contract.Requires(socket != null);
            Contract.Requires(buffer != null);
            Contract.Requires(size >= 0);
            Contract.Requires(buffer.Length >= size);

            return SendTaskAsync(socket, buffer, 0, size, socketFlags);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer, SocketFlags socketFlags)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);

            return SendTaskAsync(socket, buffer, 0, buffer.Length, socketFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, SendSokectAsyncContext context, byte[] buffer, SocketFlags socketFlags)
        {
            Contract.Requires(socket != null);
            Contract.Requires(buffer != null);
            Contract.Requires(context!=null);

            return SendTaskAsync(socket,context, buffer, 0, buffer.Length, socketFlags);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);

            return SendTaskAsync(socket, buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, SendSokectAsyncContext context, byte[] buffer)
        {
            Contract.Requires(socket != null);
            Contract.Requires(buffer != null);
            Contract.Requires(context!=null);

            return SendTaskAsync(socket,context, buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// SendSocketAsyncContext を使用して Socket.Send( buffer, size ) を TaskAsync します。
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(this Socket socket, SendSokectAsyncContext context,
                                              byte[] buffer, int size)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(context!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(size>=0);
            Contract.Requires(buffer.Length>=size);

            return SendTaskAsync(socket, context, buffer, 0, size, SocketFlags.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> SendTaskAsync(Socket socket, SendSokectAsyncContext context, byte[] buffer, int offset, int count, SocketFlags socketFlags)
        {
            Contract.Requires(socket != null);
            Contract.Requires(context != null);
            Contract.Requires(buffer != null);
            Contract.Requires(count >= 0);
            Contract.Requires(offset>=0);
            Contract.Requires(buffer.Length >= offset+count);

            var completionSource = new TaskCompletionSource<int>();
            context.CompletionSource = completionSource;
            SocketAsyncEventArgs args = context.EventArgs;
            args.SocketFlags = socketFlags;
            args.SetBuffer(buffer, offset, count);
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
        /// <param name="size"></param>
        /// <param name="socketFlags"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public static Task<int> SendToTaskAsync(this Socket socket, byte[] buffer, int size, SocketFlags socketFlags,
                                                EndPoint remoteEP)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(buffer.Length>=size);

            return SendToTaskAsync(socket, buffer, 0, size, socketFlags, remoteEP);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public static Task<int> SendToTaskAsync(this Socket socket, byte[] buffer, SocketFlags socketFlags,
                                                EndPoint remoteEP)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);

            return SendToTaskAsync(socket, buffer, 0, buffer.Length, socketFlags, remoteEP);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public static Task<int> SendToTaskAsync(this Socket socket, byte[] buffer, EndPoint remoteEP)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);

            return SendToTaskAsync(socket, buffer, 0, buffer.Length, SocketFlags.None, remoteEP);
        }

        /// <summary>
        /// Socket.SendTo( buffer, offset, count, socketFlags, remoteEP ) を Task Async 化します。
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public static Task<int> ReceiveTaskAsync(this Socket socket, byte[] buffer, int offset, int count,
                                                 SocketFlags socketFlags)
        {
            Contract.Requires(socket!=null);
            Contract.Requires(buffer!=null);
            Contract.Requires(offset>=0);
            Contract.Requires(count>=0);
            Contract.Requires(buffer.Length>=offset);
            Contract.Requires(buffer.Length>=offset+count);

            var taskCompletionSource = new TaskCompletionSource<int>();
            var socketAsyncArgs = new SocketAsyncEventArgs {SocketFlags = socketFlags};
            socketAsyncArgs.SetBuffer(buffer,offset,count);

            var onCompleted = CreateHandler(taskCompletionSource, e => e.BytesTransferred);
            if (!socket.ReceiveAsync(socketAsyncArgs))
            {
                onCompleted(socket, socketAsyncArgs);
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
