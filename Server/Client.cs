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
            ReceiveNewUserId();
            startChat = DateTime.Now;
        }
        public async Task Send(string Message)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(Message);
                await stream.WriteAsync(message, 0, message.Count());
            }
            catch (Exception e)
            {
                Console.WriteLine("Send Server Client Error" + e);
            }
        }
        public void Recieve()
        {
           while (true)
            {
                try
                {
                    char[] charsToTrim = { '\0' };
                    byte[] recievedMessage = new byte[256];
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
                    Message message = new Message(this, recievedMessageString.Trim(charsToTrim));
                    Server.messageQueue.Enqueue(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Receive Server Client Error" + e);
                }
            }
        }
        public void ReceiveNewUserId()
        {
            char[] charsToTrim = { '\0' };
            byte[] recievedMessage = new byte[256];
            try
            {
                Send("What is your chat id for this chat room?");
                stream.Read(recievedMessage, 0, recievedMessage.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("New User Send/Receive Server Error" + e);
            }
            string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
            UserId = recievedMessageString.Trim(charsToTrim);

        }
        public void ReceiveDifferentUserId()
        {
            try
            {
                Send("That Chat Id already exists...please choose a different name.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Different Id Send Server Error" + e);
            }
            ReceiveNewUserId();
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
