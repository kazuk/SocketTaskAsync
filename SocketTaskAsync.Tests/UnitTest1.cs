using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketTaskAsync.Extentions;

namespace SocketTaskAsync.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AcceptAsync()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp))
            {
                EndPoint localEp = new IPEndPoint(IPAddress.Loopback, 40001);
                socket.Bind(localEp);
                socket.Listen(10);
                var task = socket.AcceptTaskAsync();
                using (var connSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    var cont = task.ContinueWith(t =>
                        {
                            var acceptSocket = t.Result;
                            acceptSocket.RemoteEndPoint.Is(connSocket.LocalEndPoint);
                            acceptSocket.Close();
                        });
                    connSocket.Connect(localEp);

                    cont.Wait();
                }
            }
        }


        /// <summary>
        /// Accept 状態のソケットを破棄すると SocketError.OperationAborted の AsyncSocketErrorException が送出される
        /// </summary>
        [TestMethod]
        public void AcceptAsyncWithAcceptSocketClose()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                EndPoint localEp = new IPEndPoint(IPAddress.Loopback, 40000);
                socket.Bind(localEp);
                socket.Listen(10);
                var task = socket.AcceptTaskAsync();
                task.IsCompleted.IsFalse();
                socket.Close();
                try
                {
                    task.Wait();
                    Assert.Fail("unexpected Success");
                }
                catch (AggregateException e)
                {
                    e.InnerExceptions.Count.Is(1);
                    e.InnerExceptions[0].IsInstanceOf<AsyncSocketErrorException>()
                        .SocketError.Is(SocketError.OperationAborted);
                }
            }
        }
    }
}
