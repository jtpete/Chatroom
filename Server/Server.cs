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
        public static Dictionary<string, Client> removeClients = new Dictionary<string, Client>();
        TcpListener server;
        TcpClient clientSocket = default(TcpClient);
        SaveMessages saveThese = new SaveMessages(new Database());
        public Server()
        {
            try
            {
                Console.WriteLine("Starting Server Listener...");
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
                server.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error staring server " + e);
            }
        }
        public void Run()
        {
            Thread respondThread = new Thread(new ThreadStart(Respond));
            respondThread.Start();
            Thread acceptClientThread = new Thread(new ThreadStart(AcceptClient));
            acceptClientThread.Start();

            Parallel.Invoke(AcceptClient, Respond);

        }
        private void AcceptClient()
        {
            while (true)
            {
                try
                {
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = server.AcceptTcpClient();
                    NetworkStream stream = clientSocket.GetStream();
                    client = new Client(stream, clientSocket);
                    if (ConfirmUniqueId())
                    {
                        string clientStatus = client.NotifyStatus();
                        messageQueue.Enqueue(new Message(client, clientStatus));
                    }
                    Thread clientThread = new Thread(new ThreadStart(client.Recieve));
                    clientThread.Start();
                }
                catch (Exception e)
                {
                        Console.WriteLine("Error accepting client on server " + e);
                }
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
                        client.ReceiveDifferentUserId();
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
            while (true)
            {
                if (messageQueue.TryDequeue(out message))
                {
                    Console.WriteLine(message.Display());
                    saveThese.Save(message);
                    foreach(string person in allClients.Keys)
                    {
                        if (!message.sender.UserId.Equals(person) && !allClients[person].endChat.HasValue)
                            allClients[person].Send(message.Display());
                        if (allClients[person].endChat.HasValue)
                            removeClients.Add(client.UserId, client);
                    }
                    RemoveClientsAndNotify();
                }
                
            }
        }
        private void RemoveClientsAndNotify()
        {
            if (removeClients.Count > 0)
            {
                foreach (string person in removeClients.Keys)
                    allClients.Remove(person);
                removeClients.Clear();
            }
        }

    }
}


