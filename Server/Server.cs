using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static ConcurrentQueue<Message> messageQueue = new ConcurrentQueue<Message>();
        public static Client client;
        public static Dictionary<string, Client> allClients = new Dictionary<string, Client>();
        TcpListener server;
        public Server()
        {
            Console.WriteLine("Starting Server Listener...");
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
        }
        public void Run()
        {
            TcpClient clientSocket = default(TcpClient);
            AcceptClient(clientSocket);
            Respond();
            while (true)
            {
                try
                {
                    client.Recieve(clientSocket);
                    Respond();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
        private void AcceptClient(TcpClient clientSocket)
        {
            clientSocket = server.AcceptTcpClient();
            NetworkStream stream = clientSocket.GetStream();
            client = new Client(stream, clientSocket);
            if (ConfirmUniqueId())
            {
                string clientStatus = client.NotifyStatus();
                Console.WriteLine(clientStatus);
                messageQueue.Enqueue(new Message(client, clientStatus));
            }
        }
        private bool ConfirmUniqueId()
        {
            try
            {
                allClients.Add(client.UserId, client);
            }
            catch (Exception e)
            {
                bool uniqueUserId = false;
                while (!uniqueUserId)
                {
                    try
                    {
                        uniqueUserId = true;
                        client.UserId = client.ReceiveDifferentUserId();
                        allClients.Add(client.UserId, client);
                    }
                    catch
                    {
                        uniqueUserId = false;
                    }
                }
            }
            return true;
        }
        private void Respond()
        {
            Message message = default(Message);
            if (messageQueue.TryDequeue(out message))
            {
                Console.WriteLine(message.Display());
                client.Send(message.Display());
            }
        }
    }
}
