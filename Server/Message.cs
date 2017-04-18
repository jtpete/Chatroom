using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Message
    {
        public Client sender;
        public string Body;
        public string UserId;
        public DateTime messageTime;
        public Message(Client Sender, string Body)
        {
            sender = Sender;
            this.Body = Body;
            UserId = sender?.UserId;
            messageTime = DateTime.Now;
        }
        public string Display()
        {
            string message = $"{UserId}-{messageTime.ToString("hh:mm:ss tt")} >>> {Body}";
            return message;
        }
    }
}
