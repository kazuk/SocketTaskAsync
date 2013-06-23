using System;
using System.Net.Sockets;

namespace SocketTaskAsync
{
    public class AsyncSocketErrorException : Exception
    {
        private readonly SocketError _socketError;

        public AsyncSocketErrorException(SocketError socketError)
        {
            _socketError = socketError;
        }

        public SocketError SocketError
        {
            get { return _socketError; }
        }
    }
}