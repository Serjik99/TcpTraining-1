
using System.Threading;
using System;
using System.Net.Sockets;

namespace ClientClassNamespace
{
    public class ClientClass
    {

        private readonly string _serverAddress;
        private readonly int _port;
        private NetworkStream _stream;
        private Thread _listeningThread;
        private bool _isLisening;

        public ClientClass(string serverAdress,int port)
        {
            _serverAddress = serverAdress;
            _port = port;
        }
        public void Connect()
        { 
              
                TcpClient client = new TcpClient(_serverAddress, _port);
                _stream = client.GetStream();
                
                StartListening();
        }

        public void SendMessage(string message)
        {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                _stream.Write(data, 0, data.Length);
        }

        public event Action<string> OnMessageReceived;

        private void StartListening()
        {
            _isLisening = true;

            _listeningThread = new Thread(() => 
            {
                // todo fix infinity loop
                while(_isLisening)
                {
                    byte[] data = new byte[256];
                    Int32 bytes = _stream.Read(data, 0, data.Length);
                    // event
                    string message = System.Text.Encoding.ASCII.GetString(data);
                    OnMessageReceived?.Invoke(message);
                }
                
            });

            _listeningThread.Start();
        }
        private void StopListen()
        {
            _isLisening = false;
        }
    }
}