
using System;
using System.Text;

namespace BalatroGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new GameController();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            controller.Run();

            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}