using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SaveMessages
    {
        private Ilogger saveService;

        public SaveMessages(Ilogger saveMode)
        {
            saveService = saveMode;
        }

        public void Save(Message message)
        {
            saveService.Save(message);
        }
    }
}
