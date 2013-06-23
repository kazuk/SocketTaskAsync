using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    }
}
