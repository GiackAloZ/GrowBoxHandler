using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace CheckACDConverter
{
    public class ConnectionClient
    {
        private IPEndPoint _serverEndPoint;
        private int _port;
        private Socket _socket;

        public ConnectionClient(string ip, int port)
        {
            byte[] IP = new byte[3];
            string[] asd = ip.Split('.')
            _serverEndPoint = new IPEndPoint(new IPAddress())
        }

        private void Init()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.
        }
    }
}
