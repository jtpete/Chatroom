using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            ReceiveChatName();
            SetChatName();
            SendChatName();
        }
        public void Chat()
        {
            Thread receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();
            Send();

        }
        public async Task Send()
        {
            while (true)
            {
                try
                {
                    string messageString = UI.GetInput();
                    byte[] message = Encoding.ASCII.GetBytes(messageString);
                    await stream.WriteAsync(message, 0, message.Count());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error trying to send to server " + e);
                }
            }
        }

        public void Receive()
        {
            while (true)
            {
                try
                {
                    byte[] receivedMessage = new byte[256];
                    stream.Read(receivedMessage, 0, receivedMessage.Length);
                    string message = Encoding.ASCII.GetString(receivedMessage);
                    UI.DisplayMessage(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Receiving Servers Message " + e);
                }
            }
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
        public void ReceiveChatName()
        {
            byte[] receivedMessage = new byte[256];
            stream.Read(receivedMessage, 0, receivedMessage.Length);
            string message = Encoding.ASCII.GetString(receivedMessage);
            UI.DisplayMessage(message);
        }
        public void SendChatName()
        {
            string messageString = chatName;
            byte[] message = Encoding.ASCII.GetBytes(messageString);
            stream.Write(message, 0, message.Count());
        }
    }
}
