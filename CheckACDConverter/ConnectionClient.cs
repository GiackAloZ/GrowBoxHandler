using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace CheckACDConverter
{
    public class ConnectionClient
    {
        private IPEndPoint _serverEndPoint;
		private IPAddress _serverIp;
        private int _port;
        private Socket _socket;

        public bool IsConnected { get; set; }

        public ConnectionClient(string ip, int port)
        {
            byte[] IP = new byte[4];
			string[] asd = ip.Split('.');
			if(asd.Length == 4)
			{
				for (int i = 0; i < asd.Length; i++)
				{
					if (!byte.TryParse(asd[i], out IP[i]))
						throw new FormatException();
				}
			}
			else
			{
				throw new FormatException();
			}

            IsConnected = false;

			_serverIp = new IPAddress(IP);
			_port = port;
			Init();
        }

		private void Init()
		{
			_serverEndPoint = new IPEndPoint(_serverIp, _port);
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

        public void Connect()
        {
            if (!IsConnected)
                _socket.Connect(_serverEndPoint);
            else
                throw new InvalidOperationException();
            IsConnected = true;
        }

		public void SendJson()
		{
			//TODO
		}

		public void SendString(string s)
		{
			byte[] data = Encoding.ASCII.GetBytes(s);
			SendByteArray(data);
		}

		public void SendByteArray(byte[] data)
		{
			_socket.Send(data);
		}

		public void ShutdownAndClose()
		{
            if (IsConnected)
            {
			    _socket.Shutdown(SocketShutdown.Send);
			    _socket.Dispose();
            }
            IsConnected = false;
        }
    }
}
