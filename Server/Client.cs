using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        NetworkStream stream;
        TcpClient client;
        public string UserId;
        public Nullable<DateTime> startChat;
        public Nullable<DateTime> endChat;
        public Client(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            UserId = ReceiveNewUserId();
            startChat = DateTime.Now;
        }
        public void Send(string Message)
        {
            byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
        }
        public void Recieve(TcpClient clientSocket)
        {
            char[] charsToTrim = { '\0' };
            byte[] recievedMessage = new byte[256];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
            Message message = new Message(this, recievedMessageString.Trim(charsToTrim));
            Server.messageQueue.Enqueue(message);
        }
        public string ReceiveNewUserId()
        {
            char[] charsToTrim = { '\0' };
            Send("What is your chat id for this chat room?");
            byte[] recievedMessage = new byte[256];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
            return recievedMessageString.Trim(charsToTrim);
        }
        public string ReceiveDifferentUserId()
        {
            Send("That Chat Id already exists...please choose a different name.");
            return ReceiveNewUserId();
        }
        public string NotifyStatus()
        {
            if (!endChat.HasValue)
                return "Has entered the chatroom.";
            else
                return "Has left the chatroom.";
        }
    }
}
