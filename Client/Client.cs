using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        TcpClient clientSocket;
        NetworkStream stream;
        string chatName;
        public Client(string IP, int port)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(IPAddress.Parse(IP), port);
            stream = clientSocket.GetStream();
            SyncReceive();
            SetChatName();
            SendChatName();
            SyncReceive();
        }
        public void Chat()
        {
            while (true)
            {
                Send();
                Recieve();
            }
        }

        public async Task Send()
        {
            string messageString = UI.GetInput();
            byte[] message = Encoding.ASCII.GetBytes(messageString);
            await stream.WriteAsync(message, 0, message.Count());
        }

        public async Task Recieve()
        {
            byte[] receivedMessage = new byte[256];
            await stream.ReadAsync(receivedMessage, 0, receivedMessage.Length);
            string message = Encoding.ASCII.GetString(receivedMessage);
            UI.DisplayMessage(message);
        }
        public void SyncReceive()
        {
            byte[] receivedMessage = new byte[256];
            stream.Read(receivedMessage, 0, receivedMessage.Length);
            string message = Encoding.ASCII.GetString(receivedMessage);
            UI.DisplayMessage(message);
        }
        public string SetChatName()
        {
            string response = UI.GetInput();
            if (response != "")
                chatName = response;
            else
                SetChatName();
            return chatName;
        }
        public void SendChatName()
        {
            string messageString = chatName;
            byte[] message = Encoding.ASCII.GetBytes(messageString);
            stream.Write(message, 0, message.Count());
        }
    }
}
