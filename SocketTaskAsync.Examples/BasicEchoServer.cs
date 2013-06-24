using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SocketTaskAsync.Extentions;

namespace SocketTaskAsync.Examples
{
    public class BasicEchoServer
    {
        public async Task AcceptReciveSendLoopFullSync()
        {
            var socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Bind( new IPEndPoint( IPAddress.Loopback, 40020));
            socket.Listen(10);
            do
            {
                using (var acceptSocket = await socket.AcceptTaskAsync())
                {
                    var buffer = new byte[4096];

                    var received = await acceptSocket.ReceiveTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
                    acceptSocket.Shutdown(SocketShutdown.Receive);
                    await acceptSocket.SendTaskAsync( buffer,received, SocketFlags.None);
                    acceptSocket.Shutdown(SocketShutdown.Both);
                    acceptSocket.Close();
                }
            } while (true);

// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        public async Task AcceptLoopUseContinueWith()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind( new IPEndPoint(IPAddress.Loopback, 40030));
            socket.Listen(10);
            do
            {
                await socket.AcceptTaskAsync().ContinueWith(
                    async task =>
                        {
                            using (var socketAccept = task.Result)
                            {
                                var buffer = new byte[4096];
                                var received =
                                    await socketAccept.ReceiveTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
                                socketAccept.Shutdown(SocketShutdown.Receive);
                                await socketAccept.SendTaskAsync(buffer, 0, received, SocketFlags.None);
                                socketAccept.Shutdown(SocketShutdown.Both);
                                socketAccept.Close();
                            }
                        }
                    );
            } while (true);
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        public async Task AcceptLoopReuseAcceptContext()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 40040));
            socket.Listen(10);
            var context = new AcceptSocketAsyncContext();
            do
            {
                await socket.AcceptTaskAsync(context).ContinueWith(
                    async task =>
                        {
                            using (var socketAccept = task.Result)
                            {
                                var buffer = new byte[4096];
                                var received =
                                    await socketAccept.ReceiveTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
                                socketAccept.Shutdown(SocketShutdown.Receive);
                                await socketAccept.SendTaskAsync(buffer, 0, received, SocketFlags.None);
                                socketAccept.Shutdown(SocketShutdown.Both);
                                socketAccept.Close();
                            }
                        }
                    );
            } while (true);
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns
    }
}