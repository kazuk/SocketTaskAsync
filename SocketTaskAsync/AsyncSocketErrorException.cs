using System;
using System.Net.Sockets;

namespace SocketTaskAsync
{
    /// <summary>
    /// Task Async �ł̃\�P�b�g�G���[���O�Ƃ��ĕێ����܂�
    /// </summary>
    public class AsyncSocketErrorException : Exception
    {
        private readonly SocketError _socketError;

        /// <summary>
        /// Async�ł�SoketError���\�z���܂�
        /// </summary>
        /// <param name="socketError"></param>
        public AsyncSocketErrorException(SocketError socketError)
        {
            _socketError = socketError;
        }

        /// <summary>
        /// SocketError �l���擾���܂��B
        /// </summary>
        public SocketError SocketError
        {
            get { return _socketError; }
        }
    }
}