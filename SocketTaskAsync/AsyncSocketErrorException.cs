using System;
using System.Net.Sockets;

namespace SocketTaskAsync
{
    /// <summary>
    /// Task Async でのソケットエラーを例外として保持します
    /// </summary>
    public class AsyncSocketErrorException : Exception
    {
        private readonly SocketError _socketError;

        /// <summary>
        /// AsyncでのSoketErrorを構築します
        /// </summary>
        /// <param name="socketError"></param>
        public AsyncSocketErrorException(SocketError socketError)
        {
            _socketError = socketError;
        }

        /// <summary>
        /// SocketError 値を取得します。
        /// </summary>
        public SocketError SocketError
        {
            get { return _socketError; }
        }
    }
}