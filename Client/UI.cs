using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class UI
    {
        public static void DisplayMessage(string message)
        {
            char[] charsToTrim = {'\0'};
            Console.WriteLine($"{message.Trim(charsToTrim)}");
        }
        public static string GetInput()
        {
            return Console.ReadLine();
        }
        public static void MainHeader()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║         Welcome to this Chat Room             ║");
            Console.WriteLine("║                                               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

        }
    }
}
